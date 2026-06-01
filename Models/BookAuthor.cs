using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("BookAuthor")]
    public class BookAuthor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookId { get; set; }

        public int AuthorId { get; set; }

        // Navigation
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }
    }
}