using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;

namespace MovieTicketBooking.Controllers
{
    public class AdminBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== DANH SÁCH TOÀN BỘ ĐẶT VÉ ==================
        public async Task<IActionResult> Index()
        {
            // ✅ Chỉ admin mới được vào
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Auth");

            var bookings = await _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // ================== CHI TIẾT ĐẶT VÉ ==================
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Auth");

            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // ================== XOÁ HOẶC HỦY ĐẶT VÉ ==================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
