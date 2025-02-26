using GIR_Capstone.Server.Helper;
using GIR_Capstone.Server.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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

    /// <summary>
    /// Defines the _globeDecoderService
    /// </summary>
    private readonly GlobeStatusDecoderService _globeDecoderService;

    /// <summary>
    /// Defines the _ownershipTypeDecoderService
    /// </summary>
    private readonly OwnershipTypeDecoderService _ownershipTypeDecoderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorporateRepository"/> class.
    /// </summary>
    /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
    /// <param name="globeDecoderService">The globeDecoderService<see cref="GlobeStatusDecoderService"/></param>
    /// <param name="ownershipTypeDecoderService">The ownershipTypeDecoderService<see cref="OwnershipTypeDecoderService"/></param>
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
    public async Task<List<CorporateEntityDto>> GetCorporateStructureDbAsync(string corporateId)
    {
        var corporate = await _context.Corporates!
            .Include(c => c.Entities)!
                .ThenInclude(e => e.Statuses)!  // Ensure Statuses are loaded
            .Include(c => c.Entities)!
                .ThenInclude(e => e.Ownerships)  // Ensure Ownerships are loaded
            .FirstOrDefaultAsync(c => c.StructureId == new Guid(corporateId));

        if (corporate == null)
        {
            return null!; //Throw Execption
        }

        return corporate.Entities?.Select(e => new CorporateEntityDto
        {
            Id = e.Id,
            Name = e.Name,
            Jurisdiction = e.Jurisdiction,
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

    public async Task<List<CorporateEntityDto>> GetCorporateStructureXmlAsync(string corporateId)
    {
        var corporate = await _context.CorporateStructureXML
            .OrderByDescending(x => x.DateTimeCreated)  // Sort in descending order
            .FirstOrDefaultAsync(x => x.StructureId == new Guid(corporateId));

        if (corporate == null)
            return null!;

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Async = true;
        settings.IgnoreWhitespace = true; // Ignore blank spaces
        settings.IgnoreComments = true;    // Ignore XML comments
        settings.DtdProcessing = DtdProcessing.Ignore;
        settings.ConformanceLevel = ConformanceLevel.Document;

        string xmlContent = corporate.XmlData;

        using (StringReader stringReader = new StringReader(xmlContent))
        using (XmlReader reader = XmlReader.Create(stringReader, settings))
        {
            if (reader != null)
            {
                if (reader.ReadToFollowing("CorporateStructure"))
                    return await XmlParserHelper.GetCorporateStructure(reader.ReadSubtree());
            }
        }

        return null!;
    }

    /// <summary>
    /// The BatchUpdateCorporateStructureAsync
    /// </summary>
    /// <param name="corporateId">The corporateId<see cref="string"/></param>
    /// <returns>The <see cref="Task{bool}"/></returns>
    public async Task<bool> BatchUpdateCorporateStructureAsync(string corporateId)
    {
        //Benchmarking Purposes
        //long readAsyncTime = 0;
        long readToFollowingTime = 0;
        Stopwatch stopwatch = new Stopwatch();

        var corporate = await _context.CorporateStructureXML
            .OrderByDescending(x => x.DateTimeCreated)  // Sort in descending order
            .FirstOrDefaultAsync(x => x.StructureId == new Guid(corporateId));

        if (corporate == null)
            return false;

        //DeletePreviousEntities (Temp)
        var corporateEntites = await _context.CorporateEntities
            .Where(x => x.CorporationId == new Guid(corporateId)).ExecuteDeleteAsync();

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Async = true;
        settings.IgnoreWhitespace = true; // Ignore blank spaces
        settings.IgnoreComments = true;    // Ignore XML comments
        settings.DtdProcessing = DtdProcessing.Ignore;
        settings.ConformanceLevel = ConformanceLevel.Document;

        string xmlContent = corporate.XmlData;

        /*        using (StringReader stringReader = new StringReader(corporate.XmlData))
                using (XmlReader reader = XmlReader.Create(stringReader, settings))
                {
                    if (reader != null)
                    {
                        stopwatch.Start();

                        while (await reader.ReadAsync())
                        {
                            bool endLoop = false;

                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "FilingCE":
                                        await XmlParserHelper.ReadFilingCE(reader.ReadSubtree()); //Test
                                        break;
                                    case "CorporateStructure":
                                        await XmlParserHelper.ReadCorporateStructure(reader.ReadSubtree(), corporateId, _context);
                                        endLoop = true;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (endLoop)
                                break;
                        }

                        stopwatch.Stop();
                        readAsyncTime = stopwatch.ElapsedMilliseconds;
                    }
                    else
                        return false;
                }

                stopwatch.Reset();*/

        using (StringReader stringReader = new StringReader(xmlContent))
        using (XmlReader reader = XmlReader.Create(stringReader, settings))
        {
            if (reader != null)
            {
                stopwatch.Start();

                if (reader.ReadToFollowing("CorporateStructure"))
                    await XmlParserHelper.ReadCorporateStructure(reader.ReadSubtree(), corporateId, _context);

                stopwatch.Stop();
                readToFollowingTime = stopwatch.ElapsedMilliseconds;
            }
            else
                return false;
        }

        //Console.WriteLine($"ReadAsync Execution Time: {readAsyncTime} ms");
        Console.WriteLine($"ReadToFollowing Execution Time: {readToFollowingTime} ms");

        return true;
    }


}
