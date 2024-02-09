using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Models;

public partial class HallodocContext : DbContext
{
    public HallodocContext()
    {
    }

    public HallodocContext(DbContextOptions<HallodocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=HalloDocDbCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.Users).HasConstraintName("fk_AspNetUser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
