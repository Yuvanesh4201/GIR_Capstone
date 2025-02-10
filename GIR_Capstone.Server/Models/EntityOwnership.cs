using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class EntityOwnership
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("OwnedEntity")]
    public Guid OwnedEntityId { get; set; } // Child

    [ForeignKey("OwnerEntity")]
    public Guid OwnerEntityId { get; set; } // Parent

    [Required]
    public string OwnershipType { get; set; } = String.Empty;

    [Precision(4, 1)]
    public decimal OwnershipPercentage { get; set; }

    // Navigation Properties
    public virtual CorporateEntity? OwnedEntity { get; set; }
    public virtual CorporateEntity? OwnerEntity { get; set; }

}
