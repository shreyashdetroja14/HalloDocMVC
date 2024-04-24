using System;
using System.Collections.Generic;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities.Data;

public partial class HMSContext : DbContext
{
    public HMSContext()
    {
    }

    public HMSContext(DbContextOptions<HMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=HMSDbCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("doctor_pkey");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("patient_pkey");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Patients).HasConstraintName("fk_doctor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
