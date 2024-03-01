using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("case_tag")]
public partial class CaseTag
{
    [Key]
    [Column("case_tag_id")]
    public int CaseTagId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }
}
