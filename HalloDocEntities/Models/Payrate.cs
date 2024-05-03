using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("payrate")]
public partial class Payrate
{
    [Key]
    [Column("payrate_id")]
    public int PayrateId { get; set; }

    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("payrate_category_id")]
    public int? PayrateCategoryId { get; set; }

    [Column("payrate")]
    public int? Payrate1 { get; set; }

    [Column("created_by")]
    [StringLength(128)]
    public string? CreatedBy { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column("modified_by")]
    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("PayrateCategoryId")]
    [InverseProperty("Payrates")]
    public virtual PayrateCategory? PayrateCategory { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Payrates")]
    public virtual Physician Physician { get; set; } = null!;
}
