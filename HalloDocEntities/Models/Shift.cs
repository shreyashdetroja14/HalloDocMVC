using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("shift")]
public partial class Shift
{
    [Key]
    [Column("shift_id")]
    public int ShiftId { get; set; }

    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("start_date", TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }

    [Column("is_repeat")]
    public bool IsRepeat { get; set; }

    [Column("week_days")]
    [StringLength(7)]
    public string? WeekDays { get; set; }

    [Column("repeat_upto")]
    public int? RepeatUpto { get; set; }

    [Column("created_by")]
    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Shifts")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("PhysicianId")]
    [InverseProperty("Shifts")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Shift")]
    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();
}
