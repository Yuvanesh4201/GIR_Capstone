using System.ComponentModel.DataAnnotations;

public class Corporate
{
    [Key]
    public Guid StructureId { get; set; }
    public string MneName { get; set; } = string.Empty;
    // Navigation Properties
    public virtual ICollection<CorporateEntity>? Entities { get; set; }
}