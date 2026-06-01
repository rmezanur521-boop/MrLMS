using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("Book")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? ShortDesc { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        public int? GenreId { get; set; }

        [MaxLength(150)]
        public string? Publication { get; set; }

        public int? PublishedYear { get; set; }

        public int? Pages { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Length { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Width { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Height { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int AvailableCopies { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookReview> BookReviews { get; set; } = new List<BookReview>();
        public ICollection<BookReservation> BookReservations { get; set; } = new List<BookReservation>();
    }
}