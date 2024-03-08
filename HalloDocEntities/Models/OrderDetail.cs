using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("order_details")]
public partial class OrderDetail
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("vendor_id")]
    public int? VendorId { get; set; }

    [Column("request_id")]
    public int? RequestId { get; set; }

    [Column("fax_number")]
    [StringLength(50)]
    public string? FaxNumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("business_contact")]
    [StringLength(20)]
    public string? BusinessContact { get; set; }

    [Column("prescription", TypeName = "character varying")]
    public string? Prescription { get; set; }

    [Column("no_of_refill")]
    public int? NoOfRefill { get; set; }

    [Column("created_by")]
    [StringLength(128)]
    public string? CreatedBy { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderDetails")]
    public virtual AspNetUser? CreatedByNavigation { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("OrderDetails")]
    public virtual Request? Request { get; set; }

    [ForeignKey("VendorId")]
    [InverseProperty("OrderDetails")]
    public virtual HealthProfessional? Vendor { get; set; }
}
