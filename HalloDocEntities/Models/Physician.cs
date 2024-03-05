using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("physician")]
public partial class Physician
{
    [Key]
    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("asp_net_user_id")]
    [StringLength(128)]
    public string? AspNetUserId { get; set; }

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

    [Column("medical_license")]
    [StringLength(500)]
    public string? MedicalLicense { get; set; }

    [Column("photo")]
    [StringLength(100)]
    public string? Photo { get; set; }

    [Column("admin_notes")]
    [StringLength(500)]
    public string? AdminNotes { get; set; }

    [Column("is_agreement_doc")]
    public bool? IsAgreementDoc { get; set; }

    [Column("is_background_doc")]
    public bool? IsBackgroundDoc { get; set; }

    [Column("is_training_doc")]
    public bool? IsTrainingDoc { get; set; }

    [Column("is_non_disclosure_doc")]
    public bool? IsNonDisclosureDoc { get; set; }

    [Column("address1")]
    [StringLength(500)]
    public string? Address1 { get; set; }

    [Column("address2")]
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
    public string? CreatedBy { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_by")]
    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("business_name")]
    [StringLength(100)]
    public string BusinessName { get; set; } = null!;

    [Column("business_website")]
    [StringLength(200)]
    public string BusinessWebsite { get; set; } = null!;

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("npi_number")]
    [StringLength(500)]
    public string? NpiNumber { get; set; }

    [Column("is_license_doc")]
    public bool? IsLicenseDoc { get; set; }

    [Column("signature")]
    [StringLength(100)]
    public string? Signature { get; set; }

    [Column("is_credential_doc")]
    public bool? IsCredentialDoc { get; set; }

    [Column("is_token_generate")]
    public bool? IsTokenGenerate { get; set; }

    [Column("sync_email_address")]
    [StringLength(50)]
    public string? SyncEmailAddress { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("PhysicianAspNetUsers")]
    public virtual AspNetUser? AspNetUser { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("PhysicianModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Physicians")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Physician")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogPhysicians { get; set; } = new List<RequestStatusLog>();

    [InverseProperty("TransToPhysician")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogTransToPhysicians { get; set; } = new List<RequestStatusLog>();

    [InverseProperty("Physician")]
    public virtual ICollection<RequestWiseFile> RequestWiseFiles { get; set; } = new List<RequestWiseFile>();

    [InverseProperty("Physician")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
