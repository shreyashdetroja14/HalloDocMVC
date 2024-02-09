﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Models;

[Table("region")]
public partial class Region
{
    [Key]
    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("abbreviation")]
    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<RequestClient> RequestClients { get; set; } = new List<RequestClient>();

    [InverseProperty("Region")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
