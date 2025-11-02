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
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
    }
}
