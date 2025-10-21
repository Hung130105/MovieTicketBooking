using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }

        [Required]
        public int CinemaRoomId { get; set; }

        [Required]
        public string SeatNumber { get; set; } // Format: A1, A2, B1, etc.

        [Required]
        public int Row { get; set; }

        [Required]
        public int Column { get; set; }

        public string SeatType { get; set; } // Normal, VIP, etc.

        public bool IsActive { get; set; } = true;

        public virtual CinemaRoom CinemaRoom { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
    }
}