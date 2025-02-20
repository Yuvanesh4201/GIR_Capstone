using GIR_Capstone.Server.Helper;
using GIR_Capstone.Server.Services;
using Microsoft.EntityFrameworkCore;
using System.Xml;

/// <summary>
/// Defines the <see cref="CorporateRepository" />
/// </summary>
public class CorporateRepository : ICorporateRepository
{
    /// <summary>
    /// Defines the _context
    /// </summary>
    private readonly ApplicationDbContext _context;
    private readonly GlobeStatusDecoderService _globeDecoderService; 
    private readonly OwnershipTypeDecoderService _ownershipTypeDecoderService; 

    /// <summary>
    /// Initializes a new instance of the <see cref="CorporateRepository"/> class.
    /// </summary>
    /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
    public CorporateRepository(ApplicationDbContext context, GlobeStatusDecoderService globeDecoderService, OwnershipTypeDecoderService ownershipTypeDecoderService)
    {
        _context = context;
        _globeDecoderService = globeDecoderService;
        _ownershipTypeDecoderService = ownershipTypeDecoderService;
    }

    /// <summary>
    /// The GetAllCorporatesAsync
    /// </summary>
    /// <returns>The <see cref="Task{List{CorporateDto}}"/></returns>
    public async Task<List<CorporateDto>> GetAllCorporatesAsync()
    {
        return await _context.Corporates
            .Select(c => new CorporateDto
            {
                Structure_Id = c.StructureId,
                MNEName = c.MneName,
            })
            .ToListAsync();
    }

    /// <summary>
    /// The GetCorporateStructureAsync
    /// </summary>
    /// <param name="corporateId">The corporateId<see cref="string"/></param>
    /// <returns>The <see cref="Task{List{CorporateEntityDto}}"/></returns>
    public async Task<List<CorporateEntityDto>> GetCorporateStructureAsync(string corporateId)
    {
        var corporate = await _context.Corporates!
            .Include(c => c.Entities)!
                .ThenInclude(e => e.Statuses)!  // Ensure Statuses are loaded
            .Include(c => c.Entities)!
                .ThenInclude(e => e.Ownerships)  // Ensure Ownerships are loaded
            .FirstOrDefaultAsync(c => c.StructureId.ToString() == corporateId);

        if (corporate == null)
        {
            return null!; //Throw Execption
        }

        return corporate.Entities?.Select(e => new CorporateEntityDto
        {
            Id = e.Id,
            Name = e.Name,
            Jusridiction = e.Jurisdiction,
            Tin = e.Tin,
            ParentId = e.ParentId, // might need to go
            Is_Excluded = e.Is_Excluded,
            Statuses = e.Statuses?.Select(s => _globeDecoderService.Decode(s.Status)).ToList() ?? new List<string>(),
            Ownerships = e.Ownerships?.Select(o => new OwnershipDto
            {
                OwnerEntityId = o.OwnerEntityId,
                OwnerName = corporate.Entities.First(e => e.Id == o.OwnerEntityId).Name,
                OwnershipType = _ownershipTypeDecoderService.Decode(o.OwnershipType),
                OwnershipPercentage = o.OwnershipPercentage,
            }).ToList() ?? new List<OwnershipDto>(),
            qiir_Status = e?.QIIR_Status,
        }).ToList() ?? new List<CorporateEntityDto>();
    }

    /// <summary>
    /// The BatchUpdateCorporateStructureAsync
    /// </summary>
    /// <param name="corporateId">The corporateId<see cref="string"/></param>
    /// <returns>The <see cref="Task{bool}"/></returns>
    public async Task<bool> BatchUpdateCorporateStructureAsync(string corporateId)
    {
        var corporate = await _context.CorporateStructureXML
            .OrderByDescending(x => x.DateTimeCreated)  // Sort in descending order
            .FirstOrDefaultAsync(x => x.StructureId.ToString() == corporateId);

        if (corporate == null)
            return false;

        //DeletePreviousEntities (Temp)
        var corporateEntites = await _context.CorporateEntities
            .Where(x => x.CorporationId.ToString() == corporateId).ToListAsync();

        if (corporateEntites.Any())
        {
            _context.RemoveRange(corporateEntites);
            await _context.SaveChangesAsync();
        }

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Async = true;
        settings.IgnoreWhitespace = true; // Ignore blank spaces
        settings.IgnoreComments = true;    // Ignore XML comments

        using (StringReader stringReader = new StringReader(corporate.XmlData))
        using (XmlReader reader = XmlReader.Create(stringReader, settings))
        {
            if (reader != null)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "FilingCE":
                                await XmlParserHelper.ReadFilingCE(reader.ReadSubtree()); //Test
                                break;
                            case "CorporateStructure":
                                await XmlParserHelper.ReadCorporateStructure(reader.ReadSubtree(), corporateId, _context);
                                break;
                        }
                    }
                }
            }
            else
                return false;

        }

        return true;
    }
}
