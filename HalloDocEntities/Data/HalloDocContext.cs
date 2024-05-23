using System;
using System.Collections.Generic;
using HalloDocEntities.Models;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Data;

public partial class HalloDocContext : DbContext
{
    public HalloDocContext()
    {
    }

    public HalloDocContext(DbContextOptions<HalloDocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminRegion> AdminRegions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<BlockRequest> BlockRequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<CaseTag> CaseTags { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<EncounterForm> EncounterForms { get; set; }

    public virtual DbSet<HealthProfessionType> HealthProfessionTypes { get; set; }

    public virtual DbSet<HealthProfessional> HealthProfessionals { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MessageDetail> MessageDetails { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payrate> Payrates { get; set; }

    public virtual DbSet<PayrateCategory> PayrateCategories { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianLocation> PhysicianLocations { get; set; }

    public virtual DbSet<PhysicianRegion> PhysicianRegions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestBusiness> RequestBusinesses { get; set; }

    public virtual DbSet<RequestClient> RequestClients { get; set; }

    public virtual DbSet<RequestNote> RequestNotes { get; set; }

    public virtual DbSet<RequestStatusLog> RequestStatusLogs { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<RequestWiseFile> RequestWiseFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<ShiftDetailRegion> ShiftDetailRegions { get; set; }

    public virtual DbSet<SmsLog> SmsLogs { get; set; }

    public virtual DbSet<Timesheet> Timesheets { get; set; }

    public virtual DbSet<TimesheetDetail> TimesheetDetails { get; set; }

    public virtual DbSet<TimesheetReceipt> TimesheetReceipts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=HalloDocDbCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("admin_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.AdminAspNetUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_asp_net_user");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AdminModifiedByNavigations).HasConstraintName("fk_modified_by");

            entity.HasOne(d => d.Region).WithMany(p => p.Admins).HasConstraintName("fk_region");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins).HasConstraintName("fk_role");
        });

        modelBuilder.Entity<AdminRegion>(entity =>
        {
            entity.HasKey(e => e.AdminRegionId).HasName("admin_region_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin");

            entity.HasOne(d => d.Region).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_region");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asp_net_roles_pkey");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asp_net_users_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => e.AspNetUserRoleId).HasName("asp_net_user_roles_pkey");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_asp_net_user");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_asp_net_role");
        });

        modelBuilder.Entity<BlockRequest>(entity =>
        {
            entity.HasKey(e => e.BlockRequestId).HasName("block_requests_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.BlockRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.BusinessId).HasName("business_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Region).WithMany(p => p.Businesses).HasConstraintName("fk_region");
        });

        modelBuilder.Entity<CaseTag>(entity =>
        {
            entity.HasKey(e => e.CaseTagId).HasName("case_tag_pkey");
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.ConciergeId).HasName("concierge_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Region).WithMany(p => p.Concierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_region");
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.EmailLogId).HasName("email_log_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Admin).WithMany(p => p.EmailLogs).HasConstraintName("fk_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.EmailLogs).HasConstraintName("fk_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.EmailLogs).HasConstraintName("fk_request");
        });

        modelBuilder.Entity<EncounterForm>(entity =>
        {
            entity.HasKey(e => e.EncounterFormId).HasName("encounter_form_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Request).WithMany(p => p.EncounterForms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<HealthProfessionType>(entity =>
        {
            entity.HasKey(e => e.HealthProfessionId).HasName("health_profession_type_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<HealthProfessional>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("health_professionals_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Profession).WithMany(p => p.HealthProfessionals).HasConstraintName("fk_health_profession_type");

            entity.HasOne(d => d.Region).WithMany(p => p.HealthProfessionals).HasConstraintName("fk_region");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("menu_pkey");
        });

        modelBuilder.Entity<MessageDetail>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("message_details_pkey");

            entity.Property(e => e.SentTime).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_details_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderDetails).HasConstraintName("fk_created_by");

            entity.HasOne(d => d.Request).WithMany(p => p.OrderDetails).HasConstraintName("fk_request");

            entity.HasOne(d => d.Vendor).WithMany(p => p.OrderDetails).HasConstraintName("fk_vendor");
        });

        modelBuilder.Entity<Payrate>(entity =>
        {
            entity.HasKey(e => e.PayrateId).HasName("payrate_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.PayrateCategory).WithMany(p => p.Payrates).HasConstraintName("fk_category");

            entity.HasOne(d => d.Physician).WithMany(p => p.Payrates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");
        });

        modelBuilder.Entity<PayrateCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("payrate_category_pkey");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("physician_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.PhysicianAspNetUsers).HasConstraintName("fk_asp_net_user");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PhysicianModifiedByNavigations).HasConstraintName("fk_modified_by");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicians).HasConstraintName("fk_region");

            entity.HasOne(d => d.Role).WithMany(p => p.Physicians).HasConstraintName("fk_role");
        });

        modelBuilder.Entity<PhysicianLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("physician_location_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianLocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");
        });

        modelBuilder.Entity<PhysicianRegion>(entity =>
        {
            entity.HasKey(e => e.PhysicianRegionId).HasName("physician_region_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");

            entity.HasOne(d => d.Region).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_region");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("region_pkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("request_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("fk_physician");

            entity.HasOne(d => d.RequestType).WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request_type");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("fk_user");
        });

        modelBuilder.Entity<RequestBusiness>(entity =>
        {
            entity.HasKey(e => e.RequestBusinessId).HasName("request_business_pkey");

            entity.HasOne(d => d.Business).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_business");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<RequestClient>(entity =>
        {
            entity.HasKey(e => e.RequestClientId).HasName("request_client_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.RequestClients).HasConstraintName("fk_region");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<RequestNote>(entity =>
        {
            entity.HasKey(e => e.RequestNotesId).HasName("request_notes_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestNotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("request_notes_request_id_fkey");
        });

        modelBuilder.Entity<RequestStatusLog>(entity =>
        {
            entity.HasKey(e => e.RequestStatusLogId).HasName("request_status_log_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Admin).WithMany(p => p.RequestStatusLogs).HasConstraintName("fk_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequestStatusLogPhysicians).HasConstraintName("fk_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");

            entity.HasOne(d => d.TransToPhysician).WithMany(p => p.RequestStatusLogTransToPhysicians).HasConstraintName("fk_trans_to_physician");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("request_type_pkey");
        });

        modelBuilder.Entity<RequestWiseFile>(entity =>
        {
            entity.HasKey(e => e.RequestWiseFileId).HasName("request_wise_file_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Admin).WithMany(p => p.RequestWiseFiles).HasConstraintName("fk_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequestWiseFiles).HasConstraintName("fk_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestWiseFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.RoleMenuId).HasName("role_menu_pkey");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_menu");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("shift_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");
            entity.Property(e => e.WeekDays).IsFixedLength();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_created_by");

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailId).HasName("shift_detail_pkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ShiftDetails).HasConstraintName("fk_modified_by");

            entity.HasOne(d => d.Shift).WithMany(p => p.ShiftDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shift");
        });

        modelBuilder.Entity<ShiftDetailRegion>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailRegionId).HasName("shift_detail_region_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_region");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shift_detail");
        });

        modelBuilder.Entity<SmsLog>(entity =>
        {
            entity.HasKey(e => e.SmsLogId).HasName("sms_log_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Admin).WithMany(p => p.SmsLogs).HasConstraintName("fk_admin");

            entity.HasOne(d => d.Physician).WithMany(p => p.SmsLogs).HasConstraintName("fk_physician");

            entity.HasOne(d => d.Request).WithMany(p => p.SmsLogs).HasConstraintName("fk_request");
        });

        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasKey(e => e.TimesheetId).HasName("timesheet_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Physician).WithMany(p => p.Timesheets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");
        });

        modelBuilder.Entity<TimesheetDetail>(entity =>
        {
            entity.HasKey(e => e.TimesheetDetailId).HasName("timesheet_detail_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_timesheet");
        });

        modelBuilder.Entity<TimesheetReceipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("timesheet_receipts_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("LOCALTIMESTAMP");

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetReceipts).HasConstraintName("fk_timesheet");
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
