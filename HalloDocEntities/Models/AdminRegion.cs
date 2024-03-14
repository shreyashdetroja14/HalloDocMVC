using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("admin_region")]
public partial class AdminRegion
{
    [Key]
    [Column("admin_region_id")]
    public int AdminRegionId { get; set; }

    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("region_id")]
    public int RegionId { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("AdminRegions")]
    public virtual Admin Admin { get; set; } = null!;

    [ForeignKey("RegionId")]
    [InverseProperty("AdminRegions")]
    public virtual Region Region { get; set; } = null!;
}
