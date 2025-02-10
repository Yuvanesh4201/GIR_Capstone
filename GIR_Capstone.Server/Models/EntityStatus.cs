using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class EntityStatus
{
    [Key, Column(Order = 0)]
    [ForeignKey("CorporateEntity")]
    public Guid EntityId { get; set; }

    [Key, Column(Order = 1)]
    [Required]
    public string Status { get; set; } = string.Empty; // "UPE", "CE", "POPE", "JV", etc.
    
    // Navigation Property
    public virtual CorporateEntity? CorporateEntity { get; set; }
}
