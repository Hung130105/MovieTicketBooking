using Microsoft.AspNetCore.Mvc;

namespace MovieTicketBooking.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
