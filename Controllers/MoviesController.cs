using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBooking.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies với tìm kiếm - CHỈ 1 ACTION INDEX DUY NHẤT
        public async Task<IActionResult> Index(string searchString)
        {
            var movies = from m in _context.Movies
                         where m.IsActive
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString) ||
                                         m.Genre.Contains(searchString) ||
                                         m.Director.Contains(searchString));
                ViewBag.CurrentFilter = searchString;
            }

            var movieList = await movies
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();

            return View(movieList);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            var showtimes = await _context.Showtimes
                .Include(s => s.CinemaRoom)
                .Where(s => s.MovieId == id && s.StartTime.Date == today && s.IsActive)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            if (!showtimes.Any())
            {
                // Lấy tất cả phòng chiếu
                var rooms = await _context.CinemaRooms.Where(r => r.IsActive).ToListAsync();

                if (rooms.Any())
                {
                    var newShowtimes = new List<Showtime>
                    {
                        new Showtime
                        {
                            MovieId = id.Value,
                            CinemaRoomId = rooms[0].Id,
                            StartTime = today.AddHours(10), // 10:00
                            EndTime = today.AddHours(10).AddMinutes(movie.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = id.Value,
                            CinemaRoomId = rooms[1].Id,
                            StartTime = today.AddHours(14), // 14:00
                            EndTime = today.AddHours(14).AddMinutes(movie.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = id.Value,
                            CinemaRoomId = rooms[2].Id,
                            StartTime = today.AddHours(18), // 18:00
                            EndTime = today.AddHours(18).AddMinutes(movie.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = id.Value,
                            CinemaRoomId = rooms[3].Id,
                            StartTime = today.AddHours(21), // 21:00
                            EndTime = today.AddHours(21).AddMinutes(movie.Duration),
                            IsActive = true
                        }
                    };

                            _context.Showtimes.AddRange(newShowtimes);
                            await _context.SaveChangesAsync();

                            // Lấy lại danh sách showtimes sau khi thêm
                            showtimes = await _context.Showtimes
                                .Include(s => s.CinemaRoom)
                                .Where(s => s.MovieId == id && s.StartTime.Date == today && s.IsActive)
                                .OrderBy(s => s.StartTime)
                                .ToListAsync();
                        }
                    }

                    ViewBag.Showtimes = showtimes;
                    return View(movie);
                }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.IsActive = true;
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                movie.IsActive = false;
                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> GetSearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new { });
            }

            var suggestions = await _context.Movies
                .Where(m => m.IsActive &&
                           (m.Title.Contains(term) || m.Genre.Contains(term)))
                .Select(m => new {
                    id = m.Id,
                    title = m.Title,
                    poster = m.PosterUrl
                })
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}