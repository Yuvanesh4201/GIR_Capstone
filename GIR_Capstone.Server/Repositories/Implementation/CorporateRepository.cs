using System.Collections.Generic;
using System.Linq;

public class CorporateRepository : ICorporateRepository
{
    private readonly ApplicationDbContext _context;

    public CorporateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Corporate> GetAllCorporates()
    {
        return _context.Corporates.ToList();
    }
}
