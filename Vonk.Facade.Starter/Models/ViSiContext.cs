using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Vonk.Facade.Starter.Models
{
    public partial class ViSiContext : DbContext
    {
        public virtual DbSet<ViSiBloodPressure> BloodPressure { get; set; }
        public virtual DbSet<ViSiPatient> Patient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"MultipleActiveResultSets=true;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ViSi;Data Source=DUSSEL\SQL2016");
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
