using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("health_professionals")]
public partial class HealthProfessional
{
    [Key]
    [Column("vendor_id")]
    public int VendorId { get; set; }

    [Column("vendor_name")]
    [StringLength(100)]
    public string VendorName { get; set; } = null!;

    [Column("profession_id")]
    public int? ProfessionId { get; set; }

    [Column("fax_number")]
    [StringLength(50)]
    public string? FaxNumber { get; set; }

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(100)]
    public string? State { get; set; }

    [Column("zip_code")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("business_contact")]
    [StringLength(20)]
    public string? BusinessContact { get; set; }

    [InverseProperty("Vendor")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("ProfessionId")]
    [InverseProperty("HealthProfessionals")]
    public virtual HealthProfessionType? Profession { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("HealthProfessionals")]
    public virtual Region? Region { get; set; }
}
