using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("MemberSubscription")]
    public class MemberSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public int? SubId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? AmountPaid { get; set; }

        public int? BookingCount { get; set; }

        public int? BookingTime { get; set; }

        [MaxLength(20)]
        public string? PaymentMode { get; set; }

        public DateTime? PayDate { get; set; }

        // Navigation
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        [ForeignKey("SubId")]
        public Subscription? Subscription { get; set; }
    }
}