using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Models;

[Table("business")]
public partial class Business
{
    [Key]
    [Column("business_id")]
    public int BusinessId { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("address1")]
    [StringLength(500)]
    public string? Address1 { get; set; }

    [Column("address2")]
    [StringLength(500)]
    public string? Address2 { get; set; }

    [Column("city")]
    [StringLength(50)]
    public string? City { get; set; }

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("zip_code")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("fax_number")]
    [StringLength(20)]
    public string? FaxNumber { get; set; }

    [Column("is_registered")]
    public bool? IsRegistered { get; set; }

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

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Businesses")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Business")]
    public virtual ICollection<RequestBusiness> RequestBusinesses { get; set; } = new List<RequestBusiness>();
}
