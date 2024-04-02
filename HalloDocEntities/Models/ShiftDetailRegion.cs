using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("shift_detail_region")]
public partial class ShiftDetailRegion
{
    [Key]
    [Column("shift_detail_region_id")]
    public int ShiftDetailRegionId { get; set; }

    [Column("shift_detail_id")]
    public int ShiftDetailId { get; set; }

    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("ShiftDetailRegions")]
    public virtual Region Region { get; set; } = null!;

    [ForeignKey("ShiftDetailId")]
    [InverseProperty("ShiftDetailRegions")]
    public virtual ShiftDetail ShiftDetail { get; set; } = null!;
}
