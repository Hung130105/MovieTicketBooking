using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int ShowtimeId { get; set; }
        public int? UserId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } 

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public string BookingCode { get; set; }

        public virtual Showtime Showtime { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
    }
}