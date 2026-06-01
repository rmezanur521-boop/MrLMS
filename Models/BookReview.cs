using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("BookReview")]
    public class BookReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public int? BookId { get; set; }

        [Range(1, 5)]
        public int? Ratings { get; set; }

        public string? Comments { get; set; }

        public bool? IsRecommended { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime ReviewedOn { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}