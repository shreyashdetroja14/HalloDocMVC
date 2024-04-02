using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("shift_detail")]
public partial class ShiftDetail
{
    [Key]
    [Column("shift_detail_id")]
    public int ShiftDetailId { get; set; }

    [Column("shift_id")]
    public int ShiftId { get; set; }

    [Column("shift_date", TypeName = "timestamp without time zone")]
    public DateTime ShiftDate { get; set; }

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("modified_by")]
    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("last_running_date", TypeName = "timestamp without time zone")]
    public DateTime? LastRunningDate { get; set; }

    [Column("event_id")]
    [StringLength(100)]
    public string? EventId { get; set; }

    [Column("is_sync")]
    public bool? IsSync { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("ShiftDetails")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("ShiftId")]
    [InverseProperty("ShiftDetails")]
    public virtual Shift Shift { get; set; } = null!;

    [InverseProperty("ShiftDetail")]
    public virtual ICollection<ShiftDetailRegion> ShiftDetailRegions { get; set; } = new List<ShiftDetailRegion>();
}
