using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class CorporateRepository : ICorporateRepository
{
    private readonly ApplicationDbContext _context;

    public CorporateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Corporate>> GetAllCorporatesAsync()
    {
        return await _context.Corporates.ToListAsync();
    }
}
