using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("request_business")]
public partial class RequestBusiness
{
    [Key]
    [Column("request_business_id")]
    public int RequestBusinessId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("business_id")]
    public int BusinessId { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("BusinessId")]
    [InverseProperty("RequestBusinesses")]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey("RequestId")]
    [InverseProperty("RequestBusinesses")]
    public virtual Request Request { get; set; } = null!;
}
