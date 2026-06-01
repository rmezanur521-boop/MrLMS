using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("Subscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? SubscriptionName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? YearFee { get; set; }

        public int? BookingCount { get; set; }

        public int? BookingTime { get; set; }

        // Navigation
        public ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
    }
}