using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("BookReservation")]
    public class BookReservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public int? BookId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? DueDate { get; set; }

        public DateOnly? ReturnedOn { get; set; }

        public bool IsComplete { get; set; } = false;

        public bool Violation { get; set; } = false;

        [MaxLength(200)]
        public string? ViolationReason { get; set; }

        [MaxLength(500)]
        public string? ViolationRemarks { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { get; set; }

        // Navigation
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}