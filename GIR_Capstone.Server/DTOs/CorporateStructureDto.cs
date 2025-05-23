public class CorporateEntityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;
    public string Tin { get; set; } = string.Empty;
    public Guid? ParentId { get; set; } // this might need to go
    public bool Is_Excluded { get; set; }
    public List<string> Statuses { get; set; } = new List<string>();
    public List<OwnershipDto> Ownerships { get; set; } = new List<OwnershipDto>();
    public string? qiir_Status { get; set; } = string.Empty;

}

public class OwnershipDto
{
    //Maybe Create an Id for internal use (Unordered Scenario)
    public Guid Id { get; set; }  
    public Guid OwnedEntityId { get; set; }
    public Guid OwnerEntityId { get; set; }
    public string OwnerName { get; set; } = String.Empty;
    public string OwnershipType { get; set; } = String.Empty;
    public decimal OwnershipPercentage { get; set; }
}