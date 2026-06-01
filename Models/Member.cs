using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("Member")]
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Mobile { get; set; }

        [MaxLength(500)]
        public string? PasswordHash { get; set; }

        public Guid? Salt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<MemberRole> MemberRoles { get; set; } = new List<MemberRole>();
        public ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
        public ICollection<BookReview> BookReviews { get; set; } = new List<BookReview>();
        public ICollection<BookReservation> BookReservations { get; set; } = new List<BookReservation>();
    }
}