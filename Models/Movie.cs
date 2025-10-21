using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int Duration { get; set; } // in minutes

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Director { get; set; }

        public string Cast { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(255)]
        public string? PosterUrl { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Showtime> Showtimes { get; set; }
        public string? TrailerUrl { get; set; }
    }
}