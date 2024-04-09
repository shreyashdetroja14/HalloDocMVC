using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("physician_location")]
public partial class PhysicianLocation
{
    [Key]
    [Column("location_id")]
    public int LocationId { get; set; }

    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("latitude")]
    [Precision(9, 6)]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 6)]
    public decimal? Longitude { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("physician_name")]
    [StringLength(50)]
    public string? PhysicianName { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianLocations")]
    public virtual Physician Physician { get; set; } = null!;
}
