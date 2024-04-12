using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("sms_log")]
public partial class SmsLog
{
    [Key]
    [Column("sms_log_id")]
    public int SmsLogId { get; set; }

    [Column("sms_template", TypeName = "character varying")]
    public string SmsTemplate { get; set; } = null!;

    [Column("mobile_number")]
    [StringLength(50)]
    public string MobileNumber { get; set; } = null!;

    [Column("confirmation_number")]
    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    [Column("file_path", TypeName = "character varying")]
    public string? FilePath { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("request_id")]
    public int? RequestId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("physician_id")]
    public int? PhysicianId { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("sent_date", TypeName = "timestamp without time zone")]
    public DateTime? SentDate { get; set; }

    [Column("is_sms_sent")]
    public bool? IsSmsSent { get; set; }

    [Column("sent_tries")]
    public int? SentTries { get; set; }

    [Column("action")]
    public int? Action { get; set; }

    [Column("recipient_name")]
    [StringLength(200)]
    public string? RecipientName { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("SmsLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("SmsLogs")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("SmsLogs")]
    public virtual Request? Request { get; set; }
}
