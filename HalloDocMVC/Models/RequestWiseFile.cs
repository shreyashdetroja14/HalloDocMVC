using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Models;

[Table("request_wise_file")]
public partial class RequestWiseFile
{
    [Key]
    [Column("request_wise_file_id")]
    public int RequestWiseFileId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("file_name")]
    [StringLength(500)]
    public string FileName { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("physician_id")]
    public int? PhysicianId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("doc_type")]
    public short? DocType { get; set; }

    [Column("is_front_side")]
    public bool? IsFrontSide { get; set; }

    [Column("is_compensation")]
    public bool? IsCompensation { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("is_finalize")]
    public bool? IsFinalize { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("is_patient_records")]
    public bool? IsPatientRecords { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestWiseFiles")]
    public virtual Request Request { get; set; } = null!;
}
