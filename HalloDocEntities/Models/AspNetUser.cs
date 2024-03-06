using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("asp_net_users")]
public partial class AspNetUser
{
    [Key]
    [Column("id")]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [Column("user_name")]
    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Column("password_hash", TypeName = "character varying")]
    public string? PasswordHash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Admin> AdminAspNetUsers { get; set; } = new List<Admin>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Admin> AdminModifiedByNavigations { get; set; } = new List<Admin>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Physician> PhysicianAspNetUsers { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
