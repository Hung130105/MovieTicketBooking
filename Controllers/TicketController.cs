using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBooking.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Download(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("🎬 MOVIE TICKET").Bold().FontSize(20);
                        col.Item().LineHorizontal(1);
                        col.Item().Text($"Mã vé: {booking.BookingCode}");
                        col.Item().Text($"Phim: {booking.Showtime.Movie.Title}");
                        col.Item().Text($"Suất chiếu: {booking.Showtime.StartTime:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Tên khách hàng: {booking.CustomerName}");
                        col.Item().Text($"Trạng thái: {booking.Status}");
                        col.Item().Text($"Tổng tiền: {booking.TotalAmount:N0} VNĐ");
                        col.Item().AlignCenter().Text("Cảm ơn bạn đã đặt vé!").Bold();
                    });
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Ve_{booking.BookingCode}.pdf");
        }
    }
}