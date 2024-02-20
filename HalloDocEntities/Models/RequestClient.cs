using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request_client")]
public partial class RequestClient
{
    [Key]
    [Column("request_client_id")]
    public int RequestClientId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("phone_number")]
    [StringLength(23)]
    public string? PhoneNumber { get; set; }

    [Column("location")]
    [StringLength(100)]
    public string? Location { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("noti_mobile")]
    [StringLength(20)]
    public string? NotiMobile { get; set; }

    [Column("noti_email")]
    [StringLength(50)]
    public string? NotiEmail { get; set; }

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("str_month")]
    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("int_year")]
    public int? IntYear { get; set; }

    [Column("int_date")]
    public int? IntDate { get; set; }

    [Column("is_mobile")]
    public bool? IsMobile { get; set; }

    [Column("street")]
    [StringLength(100)]
    public string? Street { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(100)]
    public string? State { get; set; }

    [Column("zip_code")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    [Column("communication_type")]
    public short? CommunicationType { get; set; }

    [Column("remind_reservation_count")]
    public short? RemindReservationCount { get; set; }

    [Column("remind_house_call_count")]
    public short? RemindHouseCallCount { get; set; }

    [Column("is_set_followup_sent")]
    public short? IsSetFollowupSent { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("is_reservation_reminder_sent")]
    public short? IsReservationReminderSent { get; set; }

    [Column("latitude")]
    [Precision(9, 5)]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 5)]
    public decimal? Longitude { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("RequestClients")]
    public virtual Region? Region { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestClients")]
    public virtual Request Request { get; set; } = null!;
}
