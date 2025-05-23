using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Corporate> Corporates { get; set; }
    public DbSet<CorporateEntity> CorporateEntities { get; set; }
    public DbSet<EntityOwnership> EntityOwnerships { get; set; }
    public DbSet<EntityStatus> EntityStatuses { get; set; }
    public DbSet<CodeDecodeGlobeStatus> CodeDecodeGlobeStatus { get; set; }
    public DbSet<CodeDecodeOwnershipType> CodeDecodeOwnershipType { get; set; }
    public DbSet<CorporateStructureXML> CorporateStructureXML { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Parent-Child Relationship
        modelBuilder.Entity<CorporateEntity>()
            .HasOne(e => e.ParentEntity)
            .WithMany(e => e.ChildEntities)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Entity Ownership Relationships
        modelBuilder.Entity<EntityOwnership>()
            .HasOne(o => o.OwnedEntity)
            .WithMany(e => e.Ownerships)
            .HasForeignKey(o => o.OwnedEntityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntityOwnership>()
            .HasOne(o => o.OwnerEntity)
            .WithMany()
            .HasForeignKey(o => o.OwnerEntityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite Key for EntityStatus
        modelBuilder.Entity<EntityStatus>()
            .HasKey(es => new { es.EntityId, es.Status });

        modelBuilder.Entity<CorporateStructureXML>()
            .HasOne(c => c.Corporate)
            .WithMany()
            .HasForeignKey(c => c.StructureId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CorporateStructureXML>()
            .Property(c => c.DateTimeCreated)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        base.OnModelCreating(modelBuilder);
    }
}
