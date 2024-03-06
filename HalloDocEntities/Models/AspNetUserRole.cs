using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("asp_net_user_roles")]
public partial class AspNetUserRole
{
    [Key]
    [Column("asp_net_user_role_id")]
    public int AspNetUserRoleId { get; set; }

    [Column("asp_net_user_id")]
    [StringLength(128)]
    public string AspNetUserId { get; set; } = null!;

    [Column("role_id")]
    [StringLength(128)]
    public string RoleId { get; set; } = null!;

    [ForeignKey("AspNetUserId")]
    [InverseProperty("AspNetUserRoles")]
    public virtual AspNetUser AspNetUser { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("AspNetUserRoles")]
    public virtual AspNetRole Role { get; set; } = null!;
}
