namespace MrLMS.Models.ViewModels
{
    public class BookReviewViewModel
    {
        public int ReviewId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsRecommended { get; set; }
        public DateTime ReviewedOn { get; set; }
    }
}