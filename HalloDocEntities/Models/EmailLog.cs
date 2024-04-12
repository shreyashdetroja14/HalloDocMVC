using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("email_log")]
public partial class EmailLog
{
    [Key]
    [Column("email_log_id")]
    public int EmailLogId { get; set; }

    [Column("email_template", TypeName = "character varying")]
    public string EmailTemplate { get; set; } = null!;

    [Column("subject_name")]
    [StringLength(200)]
    public string SubjectName { get; set; } = null!;

    [Column("email_id")]
    [StringLength(200)]
    public string EmailId { get; set; } = null!;

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

    [Column("is_email_sent")]
    public bool? IsEmailSent { get; set; }

    [Column("sent_tries")]
    public int? SentTries { get; set; }

    [Column("action")]
    public int? Action { get; set; }

    [Column("recipient_name")]
    [StringLength(200)]
    public string? RecipientName { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("EmailLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("EmailLogs")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("EmailLogs")]
    public virtual Request? Request { get; set; }
}
