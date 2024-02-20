using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request_type")]
public partial class RequestType
{
    [Key]
    [Column("request_type_id")]
    public int RequestTypeId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }

    [InverseProperty("RequestType")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
