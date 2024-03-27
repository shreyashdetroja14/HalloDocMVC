using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("menu")]
public partial class Menu
{
    [Key]
    [Column("menu_id")]
    public int MenuId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("account_type")]
    public short AccountType { get; set; }

    [Column("sort_order")]
    public int? SortOrder { get; set; }

    [InverseProperty("Menu")]
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
