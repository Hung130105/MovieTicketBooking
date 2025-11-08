using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using System.Linq;

namespace MovieTicketBooking.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== TRANG CHỦ HIỂN THỊ PHIM ==================
        public async Task<IActionResult> Index(string searchString, string genreFilter, string languageFilter, string sortOrder)
        {
            // Dữ liệu ban đầu
            var movies = from m in _context.Movies
                         select m;

            // 🔍 Tìm kiếm theo tên phim
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
            }

            // 🎭 Lọc theo thể loại
            if (!string.IsNullOrEmpty(genreFilter))
            {
                movies = movies.Where(m => m.Genre == genreFilter);
            }

            // 🗣️ Lọc theo ngôn ngữ
            if (!string.IsNullOrEmpty(languageFilter))
            {
                movies = movies.Where(m => m.Language == languageFilter);
            }

            // 💰 Sắp xếp theo giá vé
            ViewData["PriceSortParam"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            switch (sortOrder)
            {
                case "price_asc":
                    movies = movies.OrderBy(m => m.Price);
                    break;
                case "price_desc":
                    movies = movies.OrderByDescending(m => m.Price);
                    break;
                default:
                    movies = movies.OrderBy(m => m.Title);
                    break;
            }

            // Truyền dữ liệu tìm kiếm và lọc lại cho View
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentGenre"] = genreFilter;
            ViewData["CurrentLanguage"] = languageFilter;
            ViewData["CurrentSort"] = sortOrder;

            return View(await movies.ToListAsync());
        }

        // ================== CHI TIẾT PHIM ==================
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Showtimes)
                .ThenInclude(s => s.CinemaRoom)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var showtimes = await _context.Showtimes
                .Include(s => s.CinemaRoom)
                .Where(s => s.MovieId == id && s.IsActive && s.StartTime >= DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            if (!showtimes.Any())
            {
                var defaultRoom = await _context.CinemaRooms.FirstOrDefaultAsync();
                if (defaultRoom == null)
                {
                    defaultRoom = new CinemaRoom
                    {
                        Name = "Phòng 1",
                        Capacity = 100,
                        IsActive = true
                    };
                    _context.CinemaRooms.Add(defaultRoom);
                    await _context.SaveChangesAsync();
                }

                var generatedShowtimes = new List<Showtime>();
                for (int i = 0; i < 3; i++)
                {
                    var start = DateTime.Today.AddDays(i).AddHours(19);
                    generatedShowtimes.Add(new Showtime
                    {
                        MovieId = movie.Id,
                        CinemaRoomId = defaultRoom.Id,
                        StartTime = start,
                        EndTime = start.AddHours(2),
                        IsActive = true
                    });
                }

                _context.Showtimes.AddRange(generatedShowtimes);
                await _context.SaveChangesAsync();

                showtimes = generatedShowtimes;
            }

            var availableDates = showtimes
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            ViewBag.Showtimes = showtimes;
            ViewBag.AvailableDates = availableDates;

            return View(movie);
        }
    }
}
