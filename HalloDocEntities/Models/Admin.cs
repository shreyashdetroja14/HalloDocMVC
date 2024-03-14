using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("admin")]
public partial class Admin
{
    [Key]
    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("asp_net_user_id")]
    [StringLength(128)]
    public string AspNetUserId { get; set; } = null!;

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column("mobile")]
    [StringLength(20)]
    public string? Mobile { get; set; }

    [Column("address_1")]
    [StringLength(500)]
    public string? Address1 { get; set; }

    [Column("address_2")]
    [StringLength(500)]
    public string? Address2 { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("zip_code")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    [Column("alt_phone")]
    [StringLength(20)]
    public string? AltPhone { get; set; }

    [Column("created_by")]
    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_by")]
    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<AdminRegion> AdminRegions { get; set; } = new List<AdminRegion>();

    [ForeignKey("AspNetUserId")]
    [InverseProperty("AdminAspNetUsers")]
    public virtual AspNetUser AspNetUser { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("AdminModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Admins")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogs { get; set; } = new List<RequestStatusLog>();

    [InverseProperty("Admin")]
    public virtual ICollection<RequestWiseFile> RequestWiseFiles { get; set; } = new List<RequestWiseFile>();

    [ForeignKey("RoleId")]
    [InverseProperty("Admins")]
    public virtual Role? Role { get; set; }
}
