using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CorporateEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Corporate")]
    public Guid CorporationId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;
    public string Tin { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool Is_Excluded { get; set; }
    public string? QIIR_Status { get; set; }  


    // Navigation Properties
    public virtual Corporate? Corporate { get; set; }
    public virtual CorporateEntity? ParentEntity { get; set; }
    public virtual ICollection<CorporateEntity>? ChildEntities { get; set; }
    public virtual ICollection<EntityOwnership>? Ownerships { get; set; }
    public virtual ICollection<EntityStatus>? Statuses { get; set; }

}