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

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=HalloDocDbCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetusers_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
