using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("role")]
public partial class Role
{
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("account_type")]
    public short AccountType { get; set; }

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

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();
}
