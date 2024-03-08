using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("health_profession_type")]
public partial class HealthProfessionType
{
    [Key]
    [Column("health_profession_id")]
    public int HealthProfessionId { get; set; }

    [Column("profession_name")]
    [StringLength(50)]
    public string ProfessionName { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [InverseProperty("Profession")]
    public virtual ICollection<HealthProfessional> HealthProfessionals { get; set; } = new List<HealthProfessional>();
}
