using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace MovieTicketBooking.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // GET: Đăng nhập
        // ==============================
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? TempData["ReturnUrl"]?.ToString();
            ViewBag.Message = TempData["Message"]?.ToString();
            return View();
        }

        // ==============================
        // POST: Đăng nhập
        // ==============================
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Mã hóa mật khẩu
            var hashedPassword = HashPassword(password);

            // Tìm người dùng khớp
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword && u.IsActive);

            if (user != null)
            {
                // Lưu thông tin vào session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
                HttpContext.Session.SetString("Role", user.Role ?? "User");

                // Chuyển hướng sau khi đăng nhập
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Movies");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ==============================
        // GET: Đăng ký
        // ==============================
        public IActionResult Register()
        {
            return View();
        }

        // ==============================
        // POST: Đăng ký
        // ==============================
        [HttpPost]
        public async Task<IActionResult> Register(
            string username, string password, string email, string fullName, string phoneNumber, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ các trường bắt buộc.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Kiểm tra username trùng
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Kiểm tra email trùng
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Email đã được sử dụng.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Tạo tài khoản mới
            var user = new User
            {
                Username = username,
                Password = HashPassword(password),
                Email = email,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Role = "User",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Sau khi đăng ký thành công -> đăng nhập luôn
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            // Chuyển hướng về trang chủ hoặc returnUrl
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Movies");
        }

        // ==============================
        // Đăng xuất
        // ==============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Movies");
        }

        // ==============================
        // Hàm mã hóa mật khẩu SHA256
        // ==============================
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}