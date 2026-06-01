using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MrLMS.Models
{
    [Table("MemberRole")]
    public class MemberRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public int? RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}