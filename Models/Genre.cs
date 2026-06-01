using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("Genre")]
    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GenreName { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}