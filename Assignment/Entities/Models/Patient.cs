using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

[Table("patient")]
public partial class Patient
{
    [Key]
    [Column("patient_id")]
    public int PatientId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("doctor_id")]
    public int? DoctorId { get; set; }

    [Column("age")]
    public int? Age { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(100)]
    public string? PhoneNumber { get; set; }

    [Column("gender")]
    public int? Gender { get; set; }

    [Column("disease")]
    [StringLength(100)]
    public string? Disease { get; set; }

    [Column("specialist")]
    [StringLength(100)]
    public string? Specialist { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Patients")]
    public virtual Doctor? Doctor { get; set; }
}
