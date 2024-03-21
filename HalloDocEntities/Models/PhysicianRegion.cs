using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("physician_region")]
public partial class PhysicianRegion
{
    [Key]
    [Column("physician_region_id")]
    public int PhysicianRegionId { get; set; }

    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("region_id")]
    public int RegionId { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianRegions")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("RegionId")]
    [InverseProperty("PhysicianRegions")]
    public virtual Region Region { get; set; } = null!;
}
