﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request")]
public partial class Request
{
    [Key]
    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("request_type_id")]
    public int RequestTypeId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("phone_number")]
    [StringLength(23)]
    public string? PhoneNumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("physician_id")]
    public int? PhysicianId { get; set; }

    [Column("confirmation_number")]
    [StringLength(20)]
    public string? ConfirmationNumber { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("declined_by")]
    [StringLength(250)]
    public string? DeclinedBy { get; set; }

    [Column("is_urgent_email_sent")]
    public bool IsUrgentEmailSent { get; set; }

    [Column("last_wellness_date", TypeName = "timestamp without time zone")]
    public DateTime? LastWellnessDate { get; set; }

    [Column("is_mobile")]
    public bool? IsMobile { get; set; }

    [Column("call_type")]
    public short? CallType { get; set; }

    [Column("is_completed_by_physician")]
    public bool? IsCompletedByPhysician { get; set; }

    [Column("last_reservation_date", TypeName = "timestamp without time zone")]
    public DateTime? LastReservationDate { get; set; }

    [Column("accepted_date", TypeName = "timestamp without time zone")]
    public DateTime? AcceptedDate { get; set; }

    [Column("relation_name")]
    [StringLength(100)]
    public string? RelationName { get; set; }

    [Column("case_number")]
    [StringLength(50)]
    public string? CaseNumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("case_tag")]
    [StringLength(50)]
    public string? CaseTag { get; set; }

    [Column("case_tag_physician")]
    [StringLength(50)]
    public string? CaseTagPhysician { get; set; }

    [Column("patient_account_id")]
    [StringLength(128)]
    public string? PatientAccountId { get; set; }

    [Column("created_user_id")]
    public int? CreatedUserId { get; set; }

    [Column("is_agreement_sent")]
    public bool? IsAgreementSent { get; set; }

    [InverseProperty("Request")]
    public virtual ICollection<BlockRequest> BlockRequests { get; set; } = new List<BlockRequest>();

    [InverseProperty("Request")]
    public virtual ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();

    [InverseProperty("Request")]
    public virtual ICollection<EncounterForm> EncounterForms { get; set; } = new List<EncounterForm>();

    [InverseProperty("Request")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("PhysicianId")]
    [InverseProperty("Requests")]
    public virtual Physician? Physician { get; set; }

    [InverseProperty("Request")]
    public virtual ICollection<RequestBusiness> RequestBusinesses { get; set; } = new List<RequestBusiness>();

    [InverseProperty("Request")]
    public virtual ICollection<RequestClient> RequestClients { get; set; } = new List<RequestClient>();

    [InverseProperty("Request")]
    public virtual ICollection<RequestNote> RequestNotes { get; set; } = new List<RequestNote>();

    [InverseProperty("Request")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogs { get; set; } = new List<RequestStatusLog>();

    [ForeignKey("RequestTypeId")]
    [InverseProperty("Requests")]
    public virtual RequestType RequestType { get; set; } = null!;

    [InverseProperty("Request")]
    public virtual ICollection<RequestWiseFile> RequestWiseFiles { get; set; } = new List<RequestWiseFile>();

    [InverseProperty("Request")]
    public virtual ICollection<SmsLog> SmsLogs { get; set; } = new List<SmsLog>();

    [ForeignKey("UserId")]
    [InverseProperty("Requests")]
    public virtual User? User { get; set; }
}
