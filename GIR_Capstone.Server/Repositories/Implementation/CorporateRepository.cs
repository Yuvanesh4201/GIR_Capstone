using Microsoft.EntityFrameworkCore;

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
    /// Initializes a new instance of the <see cref="CorporateRepository"/> class.
    /// </summary>
    /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
    public CorporateRepository(ApplicationDbContext context)
    {
        _context = context;
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
            Statuses = e.Statuses?.Select(s => s.Status).ToList() ?? new List<string>(),
            Ownerships = e.Ownerships?.Select(o => new OwnershipDto
            {
                OwnerEntityId = o.OwnerEntityId,
                OwnerName = corporate.Entities.First(e => e.Id == o.OwnerEntityId).Name,
                OwnershipType = o.OwnershipType,
                OwnershipPercentage = o.OwnershipPercentage,
            }).ToList() ?? new List<OwnershipDto>(),
            qiir_Status = e?.QIIR_Status,
        }).ToList() ?? new List<CorporateEntityDto>();
    }
}
