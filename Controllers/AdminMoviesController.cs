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

        // GET: AdminMovies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
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
            if (ModelState.IsValid)
            {
                // ✅ Nếu có chọn ảnh thì lưu
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder); // đảm bảo có thư mục
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PosterFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(stream);
                    }

                    movie.PosterUrl = fileName;
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
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
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile PosterFile)
        {
            if (id != movie.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (PosterFile != null && PosterFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(PosterFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await PosterFile.CopyToAsync(fileStream);
                        }

                        movie.PosterUrl = uniqueFileName;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật phim thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    TempData["ErrorMessage"] = "Lỗi khi cập nhật phim.";
                }
            }
            return View(movie);
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
