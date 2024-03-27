using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("role_menu")]
public partial class RoleMenu
{
    [Key]
    [Column("role_menu_id")]
    public int RoleMenuId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("menu_id")]
    public int MenuId { get; set; }

    [ForeignKey("MenuId")]
    [InverseProperty("RoleMenus")]
    public virtual Menu Menu { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("RoleMenus")]
    public virtual Role Role { get; set; } = null!;
}
