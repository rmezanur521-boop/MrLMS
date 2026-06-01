namespace MrLMS.Models.ViewModels
{
    public class BookCardViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Authors { get; set; } = string.Empty;
        public string? GenreName { get; set; }
        public string? Publication { get; set; }
        public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public double AvgRating { get; set; }
        public int ReservationCount { get; set; }
        public int? Pages {  get; set; }
        public int RecommendationCount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? MemberName { get; set; }
       
    }
}
