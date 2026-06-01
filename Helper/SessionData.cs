namespace MrLMS.Helper
{
    public class SessionData
    {
        public int MemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;  // "Admin", "Librarian", "Member"
    }
}