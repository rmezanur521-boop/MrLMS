using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("Author")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string AuthorName { get; set; } = string.Empty;

        // Navigation
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}