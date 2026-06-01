using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MrLMS.Data;
using MrLMS.Models.ViewModels;

namespace MrLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel();

            // ── helpers ──────────────────────────────────────────────────
            // Pre-load author strings per book  (BookId → "A | B | C")
            var authorMap = await _db.BookAuthors
                .Include(ba => ba.Author)
                .GroupBy(ba => ba.BookId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => string.Join(" | ", g.Select(x => x.Author!.AuthorName))
                );

            // Pre-load avg ratings per book
            var ratingMap = await _db.BookReviews
                .Where(r => r.Ratings != null)
                .GroupBy(r => r.BookId)
                .ToDictionaryAsync(
                    g => g.Key!.Value,
                    g => g.Average(x => (double)x.Ratings!.Value)
                );

            // Pre-load reservation counts per book
            var reservationMap = await _db.BookReservations
                .GroupBy(r => r.BookId)
                .ToDictionaryAsync(
                    g => g.Key!.Value,
                    g => g.Count()
                );

            // Pre-load recommendation counts per book
            var recommendMap = await _db.BookReviews
                .Where(r => r.IsRecommended == true)
                .GroupBy(r => r.BookId)
                .ToDictionaryAsync(
                    g => g.Key!.Value,
                    g => g.Count()
                );

            // Local helper function: Book → BookCardViewModel
            BookCardViewModel ToCard(Models.Book b) => new BookCardViewModel
            {
                BookId = b.Id,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                Authors = authorMap.TryGetValue(b.Id, out var a) ? a : "Unknown",
                GenreName = b.Genre?.GenreName,
                Publication = b.Publication,
                ISBN = b.ISBN,
                PublishedYear = b.PublishedYear,
                AvgRating = ratingMap.TryGetValue(b.Id, out var r) ? Math.Round(r, 1) : 0,
                ReservationCount = reservationMap.TryGetValue(b.Id, out var rc) ? rc : 0,
                RecommendationCount = recommendMap.TryGetValue(b.Id, out var rm) ? rm : 0,
                CreatedOn = b.CreatedOn
            };

            // ── 1. Top 6 New Arrivals ─────────────────────────────────────
            var newArrivalBooks = await _db.Books
                .Include(b => b.Genre)
                .OrderByDescending(b => b.CreatedOn)
                .Take(6)
                .ToListAsync();

            vm.NewArrivals = newArrivalBooks.Select(ToCard).ToList();

            // ── 2. Top 6 by Average Rating ───────────────────────────────
            var topRatedIds = await _db.BookReviews
                .Where(r => r.BookId != null && r.Ratings != null)
                .GroupBy(r => r.BookId)
                .Select(g => new { BookId = g.Key!.Value, Avg = g.Average(x => (double)x.Ratings!.Value) })
                .OrderByDescending(x => x.Avg)
                .Take(6)
                .Select(x => x.BookId)
                .ToListAsync();

            var topRatedBooks = await _db.Books
                .Include(b => b.Genre)
                .Where(b => topRatedIds.Contains(b.Id))
                .ToListAsync();

            // preserve order
            vm.TopRatedBooks = topRatedIds
                .Select(id => topRatedBooks.FirstOrDefault(b => b.Id == id))
                .Where(b => b != null)
                .Select(b => ToCard(b!))
                .ToList();

            // ── 3. Top 6 by Reservation Count ────────────────────────────
            var topReservedIds = await _db.BookReservations
                .Where(r => r.BookId != null)
                .GroupBy(r => r.BookId)
                .Select(g => new { BookId = g.Key!.Value, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .Select(x => x.BookId)
                .ToListAsync();

            var topReservedBooks = await _db.Books
                .Include(b => b.Genre)
                .Where(b => topReservedIds.Contains(b.Id))
                .ToListAsync();

            vm.TopReservedBooks = topReservedIds
                .Select(id => topReservedBooks.FirstOrDefault(b => b.Id == id))
                .Where(b => b != null)
                .Select(b => ToCard(b!))
                .ToList();

            // ── 4. Top 6 Most Recommended ────────────────────────────────
            var topRecommendedIds = await _db.BookReviews
                .Where(r => r.IsRecommended == true && r.BookId != null)
                .GroupBy(r => r.BookId)
                .Select(g => new { BookId = g.Key!.Value, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .Select(x => x.BookId)
                .ToListAsync();

            var topRecommendedBooks = await _db.Books
                .Include(b => b.Genre)
                .Where(b => topRecommendedIds.Contains(b.Id))
                .ToListAsync();

            vm.TopRecommendedBooks = topRecommendedIds
                .Select(id => topRecommendedBooks.FirstOrDefault(b => b.Id == id))
                .Where(b => b != null)
                .Select(b => ToCard(b!))
                .ToList();

            // ── 5. Genre-wise Book Count ──────────────────────────────────
            vm.GenreBookCounts = await _db.Books
                .Include(b => b.Genre)
                .Where(b => b.GenreId != null)
                .GroupBy(b => new { b.GenreId, b.Genre!.GenreName })
                .Select(g => new GenreCountViewModel
                {
                    GenreId = g.Key.GenreId!.Value,
                    GenreName = g.Key.GenreName,
                    BookCount = g.Count()
                })
                .OrderBy(g => g.GenreName)
                .ToListAsync();

            // ── 6. Books Due in Next 10 Days ──────────────────────────────
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tenDays = today.AddDays(10);

            var dueSoonReservations = await _db.BookReservations
                .Include(r => r.Book).ThenInclude(b => b!.Genre)
                .Include(r => r.Member)
                .Where(r => r.IsComplete == false
                         && r.DueDate >= today
                         && r.DueDate <= tenDays)
                .OrderBy(r => r.DueDate)
                .Take(20)
                .ToListAsync();

            vm.DueSoonBooks = dueSoonReservations.Select(r => new BookCardViewModel
            {
                BookId = r.Book!.Id,
                Title = r.Book.Title,
                ImageUrl = r.Book.ImageUrl,
                Authors = authorMap.TryGetValue(r.Book.Id, out var da) ? da : "Unknown",
                GenreName = r.Book.Genre?.GenreName,
                Publication = r.Book.Publication,
                DueDate = r.DueDate,
                MemberName = r.Member?.Name
            }).ToList();

            // ── Summary Stats ─────────────────────────────────────────────
            vm.TotalBooks = await _db.Books.CountAsync();
            vm.TotalMembers = await _db.Members.CountAsync(m => m.IsActive == true);
            vm.ActiveReservations = await _db.BookReservations.CountAsync(r => r.IsComplete == false);
            vm.TotalGenres = await _db.Genres.CountAsync();

            return View(vm);
        }
    }
}