using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("timesheet")]
public partial class Timesheet
{
    [Key]
    [Column("timesheet_id")]
    public int TimesheetId { get; set; }

    [Column("start_date", TypeName = "timestamp without time zone")]
    public DateTime? StartDate { get; set; }

    [Column("end_date", TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    [Column("physician_id")]
    public int PhysicianId { get; set; }

    [Column("bonus_amount")]
    public int? BonusAmount { get; set; }

    [Column("admin_description")]
    [StringLength(100)]
    public string? AdminDescription { get; set; }

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

    [Column("is_finalized")]
    public bool? IsFinalized { get; set; }

    [Column("is_approved")]
    public bool? IsApproved { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Timesheets")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Timesheet")]
    public virtual ICollection<TimesheetDetail> TimesheetDetails { get; set; } = new List<TimesheetDetail>();

    [InverseProperty("Timesheet")]
    public virtual ICollection<TimesheetReceipt> TimesheetReceipts { get; set; } = new List<TimesheetReceipt>();
}
