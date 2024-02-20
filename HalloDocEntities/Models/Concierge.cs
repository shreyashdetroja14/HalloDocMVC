using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("concierge")]
public partial class Concierge
{
    [Key]
    [Column("concierge_id")]
    public int ConciergeId { get; set; }

    [Column("concierge_name")]
    [StringLength(100)]
    public string ConciergeName { get; set; } = null!;

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("street")]
    [StringLength(100)]
    public string Street { get; set; } = null!;

    [Column("city")]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [Column("state")]
    [StringLength(100)]
    public string State { get; set; } = null!;

    [Column("zip_code")]
    [StringLength(10)]
    public string ZipCode { get; set; } = null!;

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("role_id")]
    [StringLength(128)]
    public string? RoleId { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Concierges")]
    public virtual Region Region { get; set; } = null!;
}
