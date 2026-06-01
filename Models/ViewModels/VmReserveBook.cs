namespace MrLMS.Models.ViewModels
{
    public class VmReserveBook
    {
        // Book info (display only)
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Authors { get; set; } = string.Empty;
        public string? GenreName { get; set; }
        public string? Publication { get; set; }

        // Reservation inputs
        public DateOnly FromDate { get; set; }
        public DateOnly DueDate { get; set; }
    }
}