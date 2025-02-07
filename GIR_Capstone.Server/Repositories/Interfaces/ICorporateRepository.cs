using System.Collections.Generic;

public interface ICorporateRepository
{
    Task<List<Corporate>> GetAllCorporatesAsync();
}