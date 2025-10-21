using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class CinemaRoom
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int Rows { get; set; }

        [Required]
        public int Columns { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Showtime> Showtimes { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
    }
}