using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Controllers
{
    public class AdminMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminMoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
            return View(movies);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile? PosterFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Error = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return View(movie);
                }

                // ✅ Upload ảnh
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadFolder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(PosterFile.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(stream);
                    }

                    movie.PosterUrl = fileName;
                }

                // ✅ Gán mặc định tránh lỗi null
                movie.Genre = string.IsNullOrWhiteSpace(movie.Genre) ? "Chưa cập nhật" : movie.Genre;
                movie.Language = string.IsNullOrWhiteSpace(movie.Language) ? "Không rõ" : movie.Language;
                movie.Director = string.IsNullOrWhiteSpace(movie.Director) ? "Chưa rõ" : movie.Director;
                movie.Cast = string.IsNullOrWhiteSpace(movie.Cast) ? "Đang cập nhật" : movie.Cast;
                movie.Duration = movie.Duration <= 0 ? 90 : movie.Duration;
                if (movie.EndDate == default(DateTime))
                    movie.EndDate = movie.ReleaseDate.AddMonths(1);

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Thêm phim mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "❌ Lỗi khi thêm phim: " + ex.Message;
                return View(movie);
            }
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? PosterFile)
        {
            if (id != movie.Id)
                return NotFound();

            try
            {
                var existingMovie = await _context.Movies.FindAsync(id);
                if (existingMovie == null)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    ViewBag.Error = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return View(movie);
                }

                // ✅ Cập nhật các trường
                existingMovie.Title = movie.Title;
                existingMovie.Description = movie.Description;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.EndDate = movie.EndDate != default(DateTime)
                    ? movie.EndDate
                    : movie.ReleaseDate.AddMonths(1);
                existingMovie.Duration = movie.Duration > 0 ? movie.Duration : 90;
                existingMovie.Genre = string.IsNullOrWhiteSpace(movie.Genre) ? "Chưa cập nhật" : movie.Genre;
                existingMovie.Language = string.IsNullOrWhiteSpace(movie.Language) ? "Không rõ" : movie.Language;
                existingMovie.Director = string.IsNullOrWhiteSpace(movie.Director) ? "Chưa rõ" : movie.Director;
                existingMovie.Cast = string.IsNullOrWhiteSpace(movie.Cast) ? "Đang cập nhật" : movie.Cast;
                existingMovie.Price = movie.Price > 0 ? movie.Price : 50000;

                // ✅ Nếu có ảnh mới thì cập nhật
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadFolder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(PosterFile.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(stream);
                    }

                    existingMovie.PosterUrl = fileName;
                }

                _context.Update(existingMovie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Cập nhật phim thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "❌ Lỗi khi cập nhật phim: " + ex.Message;
                return View(movie);
            }
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie != null)
                {
                    _context.Movies.Remove(movie);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "🗑️ Xóa phim thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi khi xóa phim: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
