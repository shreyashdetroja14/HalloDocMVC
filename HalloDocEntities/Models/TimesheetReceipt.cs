using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("timesheet_receipts")]
public partial class TimesheetReceipt
{
    [Key]
    [Column("receipt_id")]
    public int ReceiptId { get; set; }

    [Column("receipt_date", TypeName = "timestamp without time zone")]
    public DateTime? ReceiptDate { get; set; }

    [Column("timesheet_id")]
    public int? TimesheetId { get; set; }

    [Column("item_name")]
    [StringLength(100)]
    public string? ItemName { get; set; }

    [Column("amount")]
    public int? Amount { get; set; }

    [Column("file_name")]
    [StringLength(500)]
    public string? FileName { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("physician_id")]
    public int? PhysicianId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [ForeignKey("TimesheetId")]
    [InverseProperty("TimesheetReceipts")]
    public virtual Timesheet? Timesheet { get; set; }
}
