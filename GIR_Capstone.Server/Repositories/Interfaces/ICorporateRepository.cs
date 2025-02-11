using System.Collections.Generic;

public interface ICorporateRepository
{
    Task<List<CorporateDto>> GetAllCorporatesAsync();
    Task<List<CorporateEntityDto>> GetCorporateStructureAsync(string corporateId);
}