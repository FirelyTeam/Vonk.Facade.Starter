using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Visi.Repository.Models
{
    public partial class ViSiContext : DbContext
    {
        private readonly IOptions<DbOptions> _dbOptionsAccessor;

        public ViSiContext(IOptions<DbOptions> dbOptionsAccessor)
        {
            _dbOptionsAccessor = dbOptionsAccessor;
        }
        public virtual DbSet<ViSiBloodPressure> BloodPressure { get; set; }
        public virtual DbSet<ViSiPatient> Patient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_dbOptionsAccessor.Value.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViSiBloodPressure>(entity =>
            {
                entity.Property(e => e.MeasuredAt).HasColumnType("datetime");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.BloodPressure)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BloodPres__Patie__25869641");
            });

            modelBuilder.Entity<ViSiPatient>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.FamilyName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PatientNumber)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
