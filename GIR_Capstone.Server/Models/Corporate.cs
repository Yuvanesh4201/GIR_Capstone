using System.ComponentModel.DataAnnotations;

public class Corporate
{
    [Key]
    public Guid StructureId { get; set; }
    public string MneName { get; set; } = string.Empty;
    public Guid XmlSubmissionId { get; set; }

    // Navigation Properties
    public virtual ICollection<CorporateEntity>? Entities { get; set; }
}