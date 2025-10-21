using MovieTicketBooking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.ViewModels
{
    public class BookingViewModel
    {
        public int ShowtimeId { get; set; }

        [Display(Name = "Phim")]
        public string MovieTitle { get; set; }

        [Display(Name = "Thời gian chiếu")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Phòng chiếu")]
        public string CinemaRoomName { get; set; }

        [Display(Name = "Số lượng vé")]
        [Range(1, 10, ErrorMessage = "Vui lòng chọn số lượng vé từ 1 đến 10.")]
        public int TicketCount { get; set; } = 1;

        [Display(Name = "Ghế đã chọn")]
        [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 ghế.")]
        public List<int> SelectedSeatIds { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        [Display(Name = "Họ tên khách hàng")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email khách hàng")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự.")]
        [Display(Name = "Số điện thoại khách hàng")]
        public string CustomerPhone { get; set; }

        [Display(Name = "Giá vé")]
        [Range(1000, 1000000, ErrorMessage = "Giá vé không hợp lệ.")]
        public decimal MoviePrice { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        public List<SeatViewModel> AvailableSeats { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; } = "Tại quầy";

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";

        public BookingViewModel()
        {
            SelectedSeatIds = new List<int>();
            AvailableSeats = new List<SeatViewModel>();
        }
    }

    public class SeatViewModel
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
    }
}