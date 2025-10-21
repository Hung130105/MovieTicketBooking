using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.ViewModels;

namespace MovieTicketBooking.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== GET: Bookings/Create ==================
        public async Task<IActionResult> Create(int showtimeId)
        {
            // ✅ Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                TempData["ReturnUrl"] = Url.Action("Create", new { showtimeId });
                TempData["Message"] = "Vui lòng đăng nhập để đặt vé";
                return RedirectToAction("Login", "Auth");
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.CinemaRoom)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                return NotFound();

            // ✅ Lấy danh sách ghế đã đặt (nếu bạn có lưu dạng SeatNumber)
            var bookedSeatNumbers = await _context.BookingDetails
                .Where(bd => bd.Booking.ShowtimeId == showtimeId)
                .Select(bd => bd.SeatNumber)
                .ToListAsync();

            // ✅ Sinh danh sách ghế
            var allSeats = GenerateSeats(showtime.CinemaRoom.Capacity);
            var availableSeats = allSeats.Select(seat => new SeatViewModel
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                Row = seat.Row,
                Number = seat.Number,
                IsAvailable = !bookedSeatNumbers.Contains(seat.SeatNumber)
            }).ToList();

            // ✅ Thông tin người dùng
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _context.Users.FindAsync(userId);

            var vm = new BookingViewModel
            {
                ShowtimeId = showtimeId,
                MovieTitle = showtime.Movie.Title,
                StartTime = showtime.StartTime,
                CinemaRoomName = showtime.CinemaRoom.Name,
                MoviePrice = showtime.Movie.Price,
                AvailableSeats = availableSeats,
                CustomerName = user?.FullName ?? "",
                CustomerEmail = user?.Email ?? "",
                CustomerPhone = user?.PhoneNumber ?? ""
            };

            return View(vm);
        }

        // ================== POST: Bookings/Create ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel viewModel)
        {
            // ✅ Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login", "Auth");

            // ✅ Kiểm tra dữ liệu
            if (viewModel.SelectedSeatIds == null || !viewModel.SelectedSeatIds.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một ghế.");
                return await ReloadView(viewModel);
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var bookingCode = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // ✅ Tạo booking
            var booking = new Booking
            {
                ShowtimeId = viewModel.ShowtimeId,
                CustomerName = viewModel.CustomerName,
                CustomerEmail = viewModel.CustomerEmail,
                CustomerPhone = viewModel.CustomerPhone,
                BookingDate = DateTime.Now,
                TotalAmount = viewModel.SelectedSeatIds.Count * viewModel.MoviePrice,
                Status = "Đặt thành công",
                PaymentStatus = "Chưa thanh toán",
                PaymentMethod = "Tại quầy",
                BookingCode = bookingCode,
                UserId = userId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // ✅ Thêm chi tiết ghế
            foreach (var seatId in viewModel.SelectedSeatIds)
            {
                var seatInfo = viewModel.AvailableSeats.FirstOrDefault(s => s.Id == seatId);
                var seatNumber = seatInfo != null ? $"{seatInfo.Row}{seatInfo.Number}" : $"Ghế {seatId}";

                _context.BookingDetails.Add(new BookingDetail
                {
                    BookingId = booking.Id,
                    SeatNumber = seatNumber,
                    Price = viewModel.MoviePrice
                });
            }

            await _context.SaveChangesAsync();

            // Nếu người dùng chọn online -> chuyển đến trang xác nhận thanh toán
            if (viewModel.PaymentMethod == "Online")
            {
                booking.PaymentMethod = "Online";
                booking.PaymentStatus = "Đã thanh toán";
                booking.Status = "Đặt thành công (Online)";
            }
            else
            {
                booking.PaymentMethod = "Tại quầy";
                booking.PaymentStatus = "Chưa thanh toán";
                booking.Status = "Đặt thành công (Thanh toán tại quầy)";
            }


            // ✅ Chuyển đến trang Đặt vé thành công
            return RedirectToAction("Success", new { id = booking.Id });
        }

        // ================== Trang “Đặt vé thành công” ==================
        public async Task<IActionResult> Success(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // ================== Xem lịch sử đặt vé ==================
        public async Task<IActionResult> MyBookings()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var bookings = await _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // ================== Reload View ==================
        private async Task<IActionResult> ReloadView(BookingViewModel viewModel)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.CinemaRoom)
                .FirstOrDefaultAsync(s => s.Id == viewModel.ShowtimeId);

            if (showtime == null) return View(viewModel);

            var bookedSeatNumbers = await _context.BookingDetails
                .Where(bd => bd.Booking.ShowtimeId == viewModel.ShowtimeId)
                .Select(bd => bd.SeatNumber)
                .ToListAsync();

            var allSeats = GenerateSeats(showtime.CinemaRoom.Capacity);
            viewModel.AvailableSeats = allSeats.Select(seat => new SeatViewModel
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                Row = seat.Row,
                Number = seat.Number,
                IsAvailable = !bookedSeatNumbers.Contains(seat.SeatNumber)
            }).ToList();

            return View("Create", viewModel);
        }

        // ================== Sinh danh sách ghế ==================
        private List<SeatViewModel> GenerateSeats(int capacity)
        {
            var seats = new List<SeatViewModel>();
            var rows = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
            var seatsPerRow = capacity / rows.Length;
            var seatId = 1;

            foreach (var row in rows)
            {
                for (int i = 1; i <= seatsPerRow; i++)
                {
                    seats.Add(new SeatViewModel
                    {
                        Id = seatId++,
                        SeatNumber = $"{row}{i}",
                        Row = row,
                        Number = i
                    });
                }
            }
            return seats;
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            booking.Status = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Message"] = "Vé đã được hủy thành công.";
            return RedirectToAction("MyBookings");
        }
    }
}