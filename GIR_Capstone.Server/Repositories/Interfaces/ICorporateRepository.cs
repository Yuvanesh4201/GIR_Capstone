using System.Collections.Generic;

public interface ICorporateRepository
{
    Task<bool> BatchUpdateCorporateStructureAsync(string corporateId);
    Task<List<CorporateDto>> GetAllCorporatesAsync();
    Task<List<CorporateEntityDto>> GetCorporateStructureDbAsync(string corporateId);
    Task<List<CorporateEntityDto>> GetCorporateStructureXmlAsync(string corporateId);
}