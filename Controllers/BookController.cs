using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrLMS.Data;
using MrLMS.Models;
using MrLMS.Models.ViewModels;

namespace MrLMS.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _db;

        public BookController(AppDbContext db)
        {
            _db = db;
        }

        // ────────────────────────────────────────────────────────────────
        // Helper: Build dropdowns (reused in GET and POST)
        // ────────────────────────────────────────────────────────────────
        private async Task PopulateDropdowns(AdvancedSearchViewModel vm)
        {
            // Authors
            var authors = await _db.Authors
                .OrderBy(a => a.AuthorName)
                .ToListAsync();

            vm.AuthorList = authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.AuthorName
            }).ToList();
            vm.AuthorList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- All Authors --"
            });

            // Genres
            var genres = await _db.Genres
                .OrderBy(g => g.GenreName)
                .ToListAsync();

            vm.GenreList = genres.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.GenreName
            }).ToList();
            vm.GenreList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- All Genres --"
            });

            // Publications (distinct from Books table)
            var publications = await _db.Books
                .Where(b => b.Publication != null && b.Publication != "")
                .Select(b => b.Publication!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            vm.PublicationList = publications.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();
            vm.PublicationList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- All Publications --"
            });
        }

        [HttpGet]
        public async Task<IActionResult> AdvancedSearch(
            int? genreId,
            string? q)
        {
            var vm = new AdvancedSearchViewModel();
            await PopulateDropdowns(vm);

            // Coming from genre-wise count on Home page
            if (genreId.HasValue)
            {
                vm.GenreId = genreId.Value;
                vm.SearchPerformed = true;
                vm = await RunSearch(vm);
                await PopulateDropdowns(vm);
            }
            // Coming from top search bar
            else if (!string.IsNullOrWhiteSpace(q))
            {
                vm.SearchText = q.Trim();
                vm.SearchPerformed = true;
                vm = await RunSearch(vm);
                await PopulateDropdowns(vm);
            }

            return View(vm);
        }

        // ────────────────────────────────────────────────────────────────
        // POST: /Book/AdvancedSearch
        // ────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdvancedSearch(AdvancedSearchViewModel vm)
        {
            // Security: clear results from model binding
            vm.Results = new List<BookSearchResultViewModel>();

            // Validate: at least one filter must be filled
            bool hasFilter =
                !string.IsNullOrWhiteSpace(vm.SearchText) ||
                vm.AuthorId.HasValue ||
                vm.GenreId.HasValue ||
                !string.IsNullOrWhiteSpace(vm.Publication) ||
                vm.MaxPageCount.HasValue ||
                vm.PublishedYear.HasValue;

            if (!hasFilter)
            {
                vm.ValidationMessage = "Please enter or select at least one filter before searching.";
                await PopulateDropdowns(vm);
                return View(vm);
            }

            // Sanitize text inputs
            if (!string.IsNullOrWhiteSpace(vm.SearchText))
                vm.SearchText = vm.SearchText.Trim();

            vm.SearchPerformed = true;
            vm = await RunSearch(vm);
            await PopulateDropdowns(vm);

            return View(vm);
        }

        // ────────────────────────────────────────────────────────────────
        // Core search logic (shared by GET and POST)
        // ────────────────────────────────────────────────────────────────
        private async Task<AdvancedSearchViewModel> RunSearch(AdvancedSearchViewModel vm)
        {
            // Start with IQueryable — filters applied conditionally
            var query = _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsQueryable();

            // Filter 1: Title or ISBN
            if (!string.IsNullOrWhiteSpace(vm.SearchText))
            {
                string term = vm.SearchText.Trim().ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(term) ||
                    (b.ISBN != null && b.ISBN.ToLower().Contains(term)));
            }

            // Filter 2: Author
            if (vm.AuthorId.HasValue)
            {
                query = query.Where(b =>
                    b.BookAuthors.Any(ba => ba.AuthorId == vm.AuthorId.Value));
            }

            // Filter 3: Genre
            if (vm.GenreId.HasValue)
            {
                query = query.Where(b => b.GenreId == vm.GenreId.Value);
            }

            // Filter 4: Publication
            if (!string.IsNullOrWhiteSpace(vm.Publication))
            {
                query = query.Where(b => b.Publication == vm.Publication);
            }

            // Filter 5: Max Page Count
            if (vm.MaxPageCount.HasValue)
            {
                query = query.Where(b => b.Pages <= vm.MaxPageCount.Value);
            }

            // Filter 6: Published Year
            if (vm.PublishedYear.HasValue)
            {
                query = query.Where(b => b.PublishedYear == vm.PublishedYear.Value);
            }

            // Execute query
            var books = await query
                .OrderBy(b => b.Title)
                .ToListAsync();

            // Project to view model
            vm.Results = books.Select(b => new BookSearchResultViewModel
            {
                BookId = b.Id,
                Title = b.Title,
                ShortDesc = b.ShortDesc,
                ImageUrl = b.ImageUrl,
                Authors = b.BookAuthors.Any()
                                ? string.Join(" | ", b.BookAuthors
                                    .Where(ba => ba.Author != null)
                                    .Select(ba => ba.Author!.AuthorName))
                                : "Unknown",
                GenreName = b.Genre?.GenreName,
                Publication = b.Publication,
                Pages = b.Pages,
                PublishedYear = b.PublishedYear,
                ISBN = b.ISBN
            }).ToList();

            return vm;
        }

        // ────────────────────────────────────────────────────────────────
        // GET: /Book/Details/5
        // ────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            // Fetch book with Genre and Authors
            var book = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            // Reservation count
            int reservationCount = await _db.BookReservations
                .CountAsync(r => r.BookId == id);

            // Build ViewModel Section 1
            var vm = new VmBookDetails
            {
                BookId = book.Id,
                Title = book.Title,
                ShortDesc = book.ShortDesc,
                ImageUrl = book.ImageUrl,
                ISBN = book.ISBN,
                GenreName = book.Genre?.GenreName,
                Publication = book.Publication,
                PublishedYear = book.PublishedYear,
                Pages = book.Pages,
                AvailableCopies = book.AvailableCopies,
                ReservationCount = reservationCount,
                Authors = book.BookAuthors.Any()
                                  ? string.Join(", ", book.BookAuthors
                                      .Where(ba => ba.Author != null)
                                      .Select(ba => ba.Author!.AuthorName))
                                  : "Unknown"
            };

            // Section 2: Fetch ALL approved reviews for stats
            var allReviews = await _db.BookReviews
                .Include(r => r.Member)
                .Where(r => r.BookId == id && r.IsApproved == true)
                .ToListAsync();

            vm.TotalReviewCount = allReviews.Count;
            vm.TotalRecommendedCount = allReviews.Count(r => r.IsRecommended == true);
            vm.RecommendationCount = vm.TotalRecommendedCount;

            vm.OverallAvgRating = allReviews.Any(r => r.Ratings != null)
                ? Math.Round(allReviews
                    .Where(r => r.Ratings != null)
                    .Average(r => (double)r.Ratings!.Value), 1)
                : 0;

            vm.AvgRating = vm.OverallAvgRating;

            // Rating breakdown 5 → 1
            vm.RatingBreakdown = new Dictionary<int, int>();
            for (int star = 5; star >= 1; star--)
                vm.RatingBreakdown[star] = allReviews.Count(r => r.Ratings == star);

            // Top 6 reviews
            vm.TopReviews = allReviews
                .OrderByDescending(r => r.ReviewedOn)
                .ThenByDescending(r => r.Ratings)
                .Take(6)
                .Select(r => new BookReviewViewModel
                {
                    ReviewId = r.Id,
                    MemberName = r.Member?.Name ?? "Anonymous",
                    Rating = r.Ratings ?? 0,
                    Comment = r.Comments,
                    IsRecommended = r.IsRecommended ?? false,
                    ReviewedOn = r.ReviewedOn
                })
                .ToList();
            // ── Section 3: Related Books ─────────────────────────────────

            // Pre-load reservation counts for ranking
            var allReservationCounts = await _db.BookReservations
                .Where(r => r.BookId != null)
                .GroupBy(r => r.BookId!.Value)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.BookId, g => g.Count);

            // Helper: project Book → BookCardViewModel with reservation count
            BookCardViewModel ToRelatedCard(Models.Book b) => new BookCardViewModel
            {
                BookId = b.Id,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                Authors = b.BookAuthors.Any()
                                   ? string.Join(" | ", b.BookAuthors
                                       .Where(ba => ba.Author != null)
                                       .Select(ba => ba.Author!.AuthorName))
                                   : "Unknown",
                GenreName = b.Genre?.GenreName,
                Publication = b.Publication,
                ReservationCount = allReservationCounts.TryGetValue(b.Id, out var rc) ? rc : 0
            };

            // 3A: Same Genre — top 3 by reservation count (exclude current book)
            if (book.GenreId.HasValue)
            {
                var sameGenre = await _db.Books
                    .Include(b => b.Genre)
                    .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                    .Where(b => b.GenreId == book.GenreId.Value && b.Id != id)
                    .ToListAsync();

                vm.SameGenreBooks = sameGenre
                    .Select(ToRelatedCard)
                    .OrderByDescending(b => b.ReservationCount)
                    .Take(3)
                    .ToList();
            }

            // 3B: Same Author — top 3 by reservation count (exclude current book)
            var currentAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList();

            if (currentAuthorIds.Any())
            {
                var sameAuthorBookIds = await _db.BookAuthors
                    .Where(ba => currentAuthorIds.Contains(ba.AuthorId) && ba.BookId != id)
                    .Select(ba => ba.BookId)
                    .Distinct()
                    .ToListAsync();

                var sameAuthorBooks = await _db.Books
                    .Include(b => b.Genre)
                    .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                    .Where(b => sameAuthorBookIds.Contains(b.Id))
                    .ToListAsync();

                vm.SameAuthorBooks = sameAuthorBooks
                    .Select(ToRelatedCard)
                    .OrderByDescending(b => b.ReservationCount)
                    .Take(3)
                    .ToList();
            }

            // 3C: Same Publication — top 3 by reservation count (exclude current book)
            if (!string.IsNullOrEmpty(book.Publication))
            {
                var samePublication = await _db.Books
                    .Include(b => b.Genre)
                    .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                    .Where(b => b.Publication == book.Publication && b.Id != id)
                    .ToListAsync();

                vm.SamePublicationBooks = samePublication
                    .Select(ToRelatedCard)
                    .OrderByDescending(b => b.ReservationCount)
                    .Take(3)
                    .ToList();
            }

            return View(vm);
        }

        // ────────────────────────────────────────────────────────────────
        // GET: /Book/Reserve/5
        // ────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Reserve(int id)
        {
            // Security: must be logged in
            if (HttpContext.Session.GetString("UserEmail") == null)
                return Redirect($"/Account/Login?returnUrl=/Book/Reserve/{id}");

            if (id <= 0)
                return BadRequest();

            var book = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            // Check if member already has an active reservation for this book
            int memberId = HttpContext.Session.GetInt32("UserId") ?? 0;

            bool alreadyReserved = await _db.BookReservations
                .AnyAsync(r => r.BookId == id
                            && r.MemberId == memberId
                            && r.IsComplete == false);

            if (alreadyReserved)
            {
                TempData["Warning"] = "You already have an active reservation for this book.";
                return RedirectToAction("Details", new { id });
            }

            // Check available copies
            if (book.AvailableCopies <= 0)
            {
                TempData["Warning"] = "Sorry, no copies are currently available for reservation.";
                return RedirectToAction("Details", new { id });
            }

            // Build Reserve ViewModel
            var vm = new VmReserveBook
            {
                BookId = book.Id,
                Title = book.Title,
                ImageUrl = book.ImageUrl,
                Authors = book.BookAuthors.Any()
                              ? string.Join(" | ", book.BookAuthors
                                  .Where(ba => ba.Author != null)
                                  .Select(ba => ba.Author!.AuthorName))
                              : "Unknown",
                GenreName = book.Genre?.GenreName,
                Publication = book.Publication,
                FromDate = DateOnly.FromDateTime(DateTime.Today),
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14))
            };

            // Get member's subscription booking time (days allowed)
            var memberSub = await _db.MemberSubscriptions
                .Where(ms => ms.MemberId == memberId
                          && ms.ToDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderByDescending(ms => ms.ToDate)
                .FirstOrDefaultAsync();

            if (memberSub != null && memberSub.BookingTime.HasValue)
                vm.DueDate = vm.FromDate.AddDays(memberSub.BookingTime.Value);

            return View(vm);
        }

        // ────────────────────────────────────────────────────────────────
        // POST: /Book/ConfirmReservation
        // ────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReservation(VmReserveBook vm)
        {
            // Security: must be logged in
            if (HttpContext.Session.GetString("UserEmail") == null)
                return Redirect($"/Account/Login?returnUrl=/Book/Reserve/{vm.BookId}");

            int memberId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (memberId == 0)
                return Redirect("/Account/Login");

            // Re-check availability
            var book = await _db.Books.FindAsync(vm.BookId);
            if (book == null)
                return NotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Warning"] = "Sorry, no copies are currently available.";
                return RedirectToAction("Details", new { id = vm.BookId });
            }

            // Validate dates
            if (vm.DueDate <= vm.FromDate)
            {
                TempData["Warning"] = "Due date must be after the from date.";
                return RedirectToAction("Reserve", new { id = vm.BookId });
            }

            // Create reservation
            var reservation = new BookReservation
            {
                MemberId = memberId,
                BookId = vm.BookId,
                FromDate = vm.FromDate,
                DueDate = vm.DueDate,
                IsComplete = false,
                Violation = false,
                LastUpdatedBy = memberId,
                LastUpdatedOn = DateTime.Now
            };

            _db.BookReservations.Add(reservation);

            // Decrease available copies
            book.AvailableCopies -= 1;

            await _db.SaveChangesAsync();

            TempData["Success"] = $"'{book.Title}' has been reserved successfully!";
            return RedirectToAction("Details", new { id = vm.BookId });
        }
    }
}