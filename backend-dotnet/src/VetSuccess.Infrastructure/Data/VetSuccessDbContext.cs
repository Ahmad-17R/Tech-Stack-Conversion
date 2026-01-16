using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data;

public class VetSuccessDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public VetSuccessDbContext(DbContextOptions<VetSuccessDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Practice> Practices { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<SMSHistory> SMSHistories { get; set; }
    public DbSet<SMSEvent> SMSEvents { get; set; }
    public DbSet<SMSTemplate> SMSTemplates { get; set; }
    public DbSet<UpdatesEmailEvent> UpdatesEmailEvents { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<PracticeSettings> PracticeSettings { get; set; }
    public DbSet<ClientPatientRelationship> ClientPatientRelationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VetSuccessDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps automatically
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                
                if (entity.Uuid == Guid.Empty)
                {
                    entity.Uuid = Guid.NewGuid();
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
