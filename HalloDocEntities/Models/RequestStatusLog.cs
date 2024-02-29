using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request_status_log")]
public partial class RequestStatusLog
{
    [Key]
    [Column("request_status_log_id")]
    public int RequestStatusLogId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("physician_id")]
    public int? PhysicianId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("trans_to_physician_id")]
    public int? TransToPhysicianId { get; set; }

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("trans_to_admin")]
    public bool? TransToAdmin { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("RequestStatusLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("RequestStatusLogPhysicians")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestStatusLogs")]
    public virtual Request Request { get; set; } = null!;

    [ForeignKey("TransToPhysicianId")]
    [InverseProperty("RequestStatusLogTransToPhysicians")]
    public virtual Physician? TransToPhysician { get; set; }
}
