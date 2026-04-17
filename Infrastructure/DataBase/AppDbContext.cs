using System.Collections.Generic;
using Domain.DbTables;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserTable> Users => Set<UserTable>();
        public DbSet<SpaceTable> Spaces => Set<SpaceTable>();
        public DbSet<SpaceUserTable> SpaceUsers => Set<SpaceUserTable>();
        public DbSet<InvitationTable> Invitations => Set<InvitationTable>();
        public DbSet<AssetTable> Assets => Set<AssetTable>();
        public DbSet<WarrantyTable> Warranties => Set<WarrantyTable>();
        public DbSet<InsuranceTable> Insurances => Set<InsuranceTable>();
        public DbSet<DocumentTable> Documents => Set<DocumentTable>();
        public DbSet<NotificationTable> Notifications => Set<NotificationTable>();
        public DbSet<InsuranceSuggestionTable> InsuranceSuggestions => Set<InsuranceSuggestionTable>();
        public DbSet<CustomTrackerTable> CustomTrackers => Set<CustomTrackerTable>();
        public DbSet<DeviceTokenTable> DeviceTokens => Set<DeviceTokenTable>();
        public DbSet<LoanTable> Loans => Set<LoanTable>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Space ierarhie (self-reference)
            modelBuilder.Entity<SpaceTable>()
            .HasOne(s => s.ParentSpace)
             .WithMany(s => s.ChildSpaces)
             .HasForeignKey(s => s.ParentSpaceId)
             .OnDelete(DeleteBehavior.Restrict);

            // SpaceUser many-to-many index
            modelBuilder.Entity<SpaceUserTable>()
                .HasIndex(su => new { su.SpaceId, su.UserId })
                .IsUnique();

            // SpaceUser -> Space (Cascade)
            modelBuilder.Entity<SpaceUserTable>()
                .HasOne(su => su.Space)
                .WithMany(s => s.SpaceUsers)
                .HasForeignKey(su => su.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // SpaceUser -> User (Restrict)
            modelBuilder.Entity<SpaceUserTable>()
                .HasOne(su => su.User)
                .WithMany(u => u.SpaceUsers)
                .HasForeignKey(su => su.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set decimal precision for AssetTable.Value
            modelBuilder.Entity<AssetTable>()
                .Property(a => a.Value)
                .HasPrecision(18, 2);

            // Set decimal precision for InsuranceTable.InsuredValue
            modelBuilder.Entity<InsuranceTable>()
                .Property(i => i.InsuredValue)
                .HasPrecision(18, 2);

            // Notification -> User (Restrict)
            modelBuilder.Entity<NotificationTable>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification -> Asset (Cascade)
            modelBuilder.Entity<NotificationTable>()
                .HasOne(n => n.Asset)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // InsuranceSuggestion -> Asset (Cascade)
            modelBuilder.Entity<InsuranceSuggestionTable>()
                .HasOne(i => i.Asset)
                .WithMany(a => a.InsuranceSuggestions)
                .HasForeignKey(i => i.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Document -> Asset (Cascade)
            modelBuilder.Entity<DocumentTable>()
                .HasOne(d => d.Asset)
                .WithMany(a => a.Documents)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Document -> Loan (optional, NoAction)
            modelBuilder.Entity<DocumentTable>()
                .HasOne(d => d.Loan)
                .WithMany()
                .HasForeignKey(d => d.LoanId)
                .OnDelete(DeleteBehavior.NoAction);

            // Insurance -> Asset (Cascade)
            modelBuilder.Entity<InsuranceTable>()
                .HasOne(i => i.Asset)
                .WithOne(a => a.Insurance) // Use WithOne for 1:1 navigation
                .HasForeignKey<InsuranceTable>(i => i.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Insurance -> Space (optional, NoAction)
            modelBuilder.Entity<InsuranceTable>()
                .HasOne(i => i.Space)
                .WithMany(s => s.Insurances)
                .HasForeignKey(i => i.SpaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Warranty -> Asset (Cascade)
            modelBuilder.Entity<WarrantyTable>()
                .HasOne(w => w.Asset)
                .WithOne(a => a.Warranty) // Use WithOne for 1:1 navigation
                .HasForeignKey<WarrantyTable>(w => w.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Warranty -> Space (optional, NoAction)
            modelBuilder.Entity<WarrantyTable>()
                .HasOne(w => w.Space)
                .WithMany(s => s.Warranties)
                .HasForeignKey(w => w.SpaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Invitation -> Space (Cascade)
            modelBuilder.Entity<InvitationTable>()
                .HasOne(inv => inv.Space)
                .WithMany(s => s.Invitations)
                .HasForeignKey(inv => inv.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asset -> Space (Cascade)
            modelBuilder.Entity<AssetTable>()
                .HasOne(a => a.Space)
                .WithMany(s => s.Assets)
                .HasForeignKey(a => a.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Space -> User (Owner) (Cascade)
            modelBuilder.Entity<SpaceTable>()
                .HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enum mapping as string (optional, for readability)
            modelBuilder.Entity<SpaceTable>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<SpaceUserTable>()
                .Property(e => e.Role)
                .HasConversion<string>();

            modelBuilder.Entity<InvitationTable>()
                .Property(e => e.Role)
                .HasConversion<string>();

            modelBuilder.Entity<DocumentTable>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<NotificationTable>()
                .Property(e => e.Type)
                .HasConversion<string>();

            // CustomTracker -> Asset (Cascade, 1:many)
            modelBuilder.Entity<CustomTrackerTable>()
                .HasOne(ct => ct.Asset)
                .WithMany(a => a.CustomTrackers)
                .HasForeignKey(ct => ct.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification -> CustomTracker (opțional, NoAction pentru a evita cascade paths multiple)
            modelBuilder.Entity<NotificationTable>()
                .HasOne(n => n.CustomTracker)
                .WithMany()
                .HasForeignKey(n => n.CustomTrackerId)
                .OnDelete(DeleteBehavior.NoAction);

            // DeviceToken -> User (Cascade)
            modelBuilder.Entity<DeviceTokenTable>()
                .HasOne(dt => dt.User)
                .WithMany(u => u.DeviceTokens)
                .HasForeignKey(dt => dt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index unic pe Token (un device = un token)
            modelBuilder.Entity<DeviceTokenTable>()
                .HasIndex(dt => dt.Token)
                .IsUnique();

            // Loan -> Asset (Cascade)
            modelBuilder.Entity<LoanTable>()
                .HasOne(l => l.Asset)
                .WithMany(a => a.Loans)
                .HasForeignKey(l => l.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
