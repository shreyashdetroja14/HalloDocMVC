using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("block_requests")]
public partial class BlockRequest
{
    [Key]
    [Column("block_request_id")]
    public int BlockRequestId { get; set; }

    [Column("phone_number")]
    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("BlockRequests")]
    public virtual Request Request { get; set; } = null!;
}
