using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("message_details")]
public partial class MessageDetail
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("sender_id")]
    [StringLength(128)]
    public string? SenderId { get; set; }

    [Column("receiver_id")]
    [StringLength(128)]
    public string? ReceiverId { get; set; }

    [Column("message_text", TypeName = "character varying")]
    public string? MessageText { get; set; }

    [Column("sent_time", TypeName = "timestamp without time zone")]
    public DateTime SentTime { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; }
}
