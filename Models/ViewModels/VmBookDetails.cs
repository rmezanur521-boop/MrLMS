using MrLMS.Models.ViewModels;

namespace MrLMS.Models.ViewModels
{
    public class VmBookDetails
    {
        // ── Section 1: Book Info ─────────────────────────────────
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDesc { get; set; }
        public string? ImageUrl { get; set; }
        public string? ISBN { get; set; }
        public string? GenreName { get; set; }
        public string? Publication { get; set; }
        public int? PublishedYear { get; set; }
        public int? Pages { get; set; }
        public string Authors { get; set; } = string.Empty;
        public int AvailableCopies { get; set; }

        // Live stats
        public int ReservationCount { get; set; }
        public int RecommendationCount { get; set; }
        public double AvgRating { get; set; }

        // ── Section 2: Reviews ───────────────────────────────────
        public List<BookReviewViewModel> TopReviews { get; set; } = new();
        public double OverallAvgRating { get; set; }
        public int TotalReviewCount { get; set; }
        public int TotalRecommendedCount { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new();

        // ── Section 3: Related Books ─────────────────────────────────
        public List<BookCardViewModel> SameGenreBooks { get; set; } = new();
        public List<BookCardViewModel> SameAuthorBooks { get; set; } = new();
        public List<BookCardViewModel> SamePublicationBooks { get; set; } = new();
    }
}