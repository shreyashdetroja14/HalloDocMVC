using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request_notes")]
public partial class RequestNote
{
    [Key]
    [Column("request_notes_id")]
    public int RequestNotesId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("str_month")]
    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("int_year")]
    public int? IntYear { get; set; }

    [Column("int_date")]
    public int? IntDate { get; set; }

    [Column("physician_notes")]
    [StringLength(500)]
    public string? PhysicianNotes { get; set; }

    [Column("admin_notes")]
    [StringLength(500)]
    public string? AdminNotes { get; set; }

    [Column("created_by")]
    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_by")]
    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("administrative_notes")]
    [StringLength(500)]
    public string? AdministrativeNotes { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestNotes")]
    public virtual Request Request { get; set; } = null!;
}
