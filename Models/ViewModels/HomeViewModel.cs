namespace MrLMS.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<BookCardViewModel> TopRatedBooks { get; set; } = new();
        public List<BookCardViewModel> TopReservedBooks { get; set; } = new();
        public List<BookCardViewModel> TopRecommendedBooks { get; set; } = new();
        public List<BookCardViewModel> NewArrivals { get; set; } = new();
        public List<BookCardViewModel> DueSoonBooks { get; set; } = new();
        public List<GenreCountViewModel> GenreBookCounts { get; set; } = new();

        // Summary stats for top bar
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveReservations { get; set; }
        public int TotalGenres { get; set; }
    }
}