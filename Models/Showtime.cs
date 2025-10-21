using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int CinemaRoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Movie Movie { get; set; }
        public virtual CinemaRoom CinemaRoom { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}