using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CorporateStructureXML
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("Corporate")]
    public Guid StructureId { get; set; }
    [Column(TypeName = "XML")]
    public string XmlData { get; set; } = string.Empty;
    public DateTimeOffset DateTimeCreated { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation Properties
    public virtual Corporate? Corporate { get; set; }
}