using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrLMS.Models.ViewModels
{
    public class AdvancedSearchViewModel
    {
        // ── Filter Inputs ────────────────────────────────────────────
        public string? SearchText { get; set; }        // Title or ISBN
        public int? AuthorId { get; set; }
        public int? GenreId { get; set; }
        public string? Publication { get; set; }
        public int? MaxPageCount { get; set; }
        public int? PublishedYear { get; set; }

        // ── Dropdowns ────────────────────────────────────────────────
        public List<SelectListItem> AuthorList { get; set; } = new();
        public List<SelectListItem> GenreList { get; set; } = new();
        public List<SelectListItem> PublicationList { get; set; } = new();

        // ── Results ──────────────────────────────────────────────────
        public List<BookSearchResultViewModel> Results { get; set; } = new();

        public bool SearchPerformed { get; set; } = false;
        public string? ValidationMessage { get; set; }
    }

    public class BookSearchResultViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDesc { get; set; }
        public string? ImageUrl { get; set; }
        public string Authors { get; set; } = string.Empty;
        public string? GenreName { get; set; }
        public string? Publication { get; set; }
        public int? Pages { get; set; }
        public int? PublishedYear { get; set; }
        public string? ISBN { get; set; }
    }
}