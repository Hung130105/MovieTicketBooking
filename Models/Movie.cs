using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phim không được để trống")]
        [StringLength(150, ErrorMessage = "Tên phim không được vượt quá 150 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả phim là bắt buộc")]
        [StringLength(2000, ErrorMessage = "Mô tả phim không được vượt quá 2000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Thời lượng phim là bắt buộc")]
        [Range(1, 600, ErrorMessage = "Thời lượng phim phải lớn hơn 0")]
        public int Duration { get; set; } // Đơn vị: phút

        [Required(ErrorMessage = "Thể loại phim là bắt buộc")]
        [StringLength(100)]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Đạo diễn là bắt buộc")]
        [StringLength(100)]
        public string Director { get; set; }

        [StringLength(300)]
        public string? Cast { get; set; }

        [Required(ErrorMessage = "Ngày khởi chiếu là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [StringLength(255)]
        public string? PosterUrl { get; set; }  // tên file ảnh lưu trong thư mục /wwwroot/images

        [Required(ErrorMessage = "Ngôn ngữ phim là bắt buộc")]
        [StringLength(50)]
        public string Language { get; set; }

        [Required(ErrorMessage = "Giá vé là bắt buộc")]
        [Range(0, 1000000, ErrorMessage = "Giá vé phải từ 0 đến 1,000,000 VNĐ")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ Khởi tạo tránh lỗi null reference
        public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        [StringLength(255)]
        public string? TrailerUrl { get; set; }
    }
}
