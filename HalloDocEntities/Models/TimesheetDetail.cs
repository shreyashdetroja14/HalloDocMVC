using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("timesheet_detail")]
public partial class TimesheetDetail
{
    [Key]
    [Column("timesheet_detail_id")]
    public int TimesheetDetailId { get; set; }

    [Column("timesheet_id")]
    public int TimesheetId { get; set; }

    [Column("timesheet_detail_date", TypeName = "timestamp without time zone")]
    public DateTime? TimesheetDetailDate { get; set; }

    [Column("on_call_hours")]
    public int? OnCallHours { get; set; }

    [Column("total_hours")]
    public int? TotalHours { get; set; }

    [Column("is_night_weekend")]
    public bool IsNightWeekend { get; set; }

    [Column("housecalls_count")]
    public int? HousecallsCount { get; set; }

    [Column("phoneconsult_count")]
    public int? PhoneconsultCount { get; set; }

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

    [ForeignKey("TimesheetId")]
    [InverseProperty("TimesheetDetails")]
    public virtual Timesheet Timesheet { get; set; } = null!;
}
