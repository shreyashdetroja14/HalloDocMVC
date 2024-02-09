﻿using System;
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

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestClient> RequestClients { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=HalloDocDbCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asp_net_users_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("region_pkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("request_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.RequestType).WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request_type");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("fk_user");
        });

        modelBuilder.Entity<RequestClient>(entity =>
        {
            entity.HasKey(e => e.RequestClientId).HasName("request_client_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.RequestClients).HasConstraintName("fk_region");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("request_type_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.Users).HasConstraintName("fk_asp_net_user");

            entity.HasOne(d => d.Region).WithMany(p => p.Users).HasConstraintName("fk_region");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
