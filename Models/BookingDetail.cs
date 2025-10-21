using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class BookingDetail
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "Số ghế")]
        public string SeatNumber { get; set; } = string.Empty; // VD: "A1", "B5"

        [Required]
        public decimal Price { get; set; }

        public virtual Booking Booking { get; set; }
    }
}