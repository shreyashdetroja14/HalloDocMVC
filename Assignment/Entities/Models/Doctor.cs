using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

[Table("doctor")]
public partial class Doctor
{
    [Key]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Column("specialist")]
    [StringLength(100)]
    public string? Specialist { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
