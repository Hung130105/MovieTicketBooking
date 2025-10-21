using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Kiểm tra nếu chưa có dữ liệu phim
            if (!context.Movies.Any())
            {
                context.Movies.AddRange(
                    new Movie
                    {
                        Title = "Avengers: Endgame",
                        Description = "Phim siêu anh hùng của Marvel - Avengers tập hợp để cứu thế giới.",
                        Duration = 180,
                        Genre = "Hành động",
                        Director = "Anthony Russo",
                        Cast = "Robert Downey Jr., Chris Evans",
                        ReleaseDate = new DateTime(2024, 1, 1),
                        EndDate = new DateTime(2024, 3, 1),
                        PosterUrl = "/images/avengers.jpg",
                        Language = "Tiếng Anh",
                        Price = 90000,
                        IsActive = true
                    },
                    new Movie
                    {
                        Title = "Inception",
                        Description = "Giấc mơ trong giấc mơ - Một tên trộm đánh cắp bí mật qua giấc mơ.",
                        Duration = 148,
                        Genre = "Khoa học viễn tưởng",
                        Director = "Christopher Nolan",
                        Cast = "Leonardo DiCaprio",
                        ReleaseDate = new DateTime(2024, 1, 15),
                        EndDate = new DateTime(2024, 3, 15),
                        PosterUrl = "/images/inception.jpg",
                        Language = "Tiếng Anh",
                        Price = 85000,
                        IsActive = true
                    },
                    new Movie
                    {
                        Title = "Dune: Part Two",
                        Description = "Cuộc chiến giành hành tinh Arrakis tiếp tục với Paul Atreides và người Fremen.",
                        Duration = 166,
                        Genre = "Khoa học viễn tưởng",
                        Director = "Denis Villeneuve",
                        Cast = "Timothée Chalamet",
                        ReleaseDate = new DateTime(2024, 3, 1),
                        EndDate = new DateTime(2024, 5, 1),
                        PosterUrl = "/images/dune2.jpg",
                        Language = "Tiếng Anh",
                        Price = 95000,
                        IsActive = true
                    },
                    new Movie
                    {
                        Title = "Kung Fu Panda 4",
                        Description = "Po trở lại để huấn luyện Chiến binh Rồng tiếp theo và đối đầu với kẻ thù mới.",
                        Duration = 95,
                        Genre = "Hoạt hình",
                        Director = "Mike Mitchell",
                        Cast = "Jack Black",
                        ReleaseDate = new DateTime(2024, 3, 8),
                        EndDate = new DateTime(2024, 5, 8),
                        PosterUrl = "/images/kungfupanda4.jpg",
                        Language = "Tiếng Anh",
                        Price = 75000,
                        IsActive = true
                    }
                );

                context.SaveChanges();
            }

            // Thêm CinemaRoom nếu chưa có
            if (!context.CinemaRooms.Any())
            {
                context.CinemaRooms.AddRange(
                    new CinemaRoom { Name = "Phòng 1 - 2D", Capacity = 100, IsActive = true },
                    new CinemaRoom { Name = "Phòng 2 - IMAX", Capacity = 150, IsActive = true },
                    new CinemaRoom { Name = "Phòng 3 - 3D", Capacity = 80, IsActive = true },
                    new CinemaRoom { Name = "Phòng 4 - VIP", Capacity = 50, IsActive = true }
                );

                context.SaveChanges();
            }

            // Thêm Showtime cho các phim, đặc biệt là Kung Fu Panda 4
            if (!context.Showtimes.Any())
            {
                var kungFuPanda = context.Movies.FirstOrDefault(m => m.Title == "Kung Fu Panda 4");
                var avengers = context.Movies.FirstOrDefault(m => m.Title == "Avengers: Endgame");
                var inception = context.Movies.FirstOrDefault(m => m.Title == "Inception");
                var dune = context.Movies.FirstOrDefault(m => m.Title == "Dune: Part Two");

                var rooms = context.CinemaRooms.ToList();

                if (kungFuPanda != null && rooms.Any())
                {
                    var today = DateTime.Today;

                    // Thêm nhiều suất chiếu cho Kung Fu Panda 4 trong ngày hôm nay
                    var kungFuShowtimes = new List<Showtime>
                    {
                        new Showtime
                        {
                            MovieId = kungFuPanda.Id,
                            CinemaRoomId = rooms[0].Id,
                            StartTime = today.AddHours(10), // 10:00
                            EndTime = today.AddHours(10).AddMinutes(kungFuPanda.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = kungFuPanda.Id,
                            CinemaRoomId = rooms[1].Id,
                            StartTime = today.AddHours(13), // 13:00
                            EndTime = today.AddHours(13).AddMinutes(kungFuPanda.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = kungFuPanda.Id,
                            CinemaRoomId = rooms[2].Id,
                            StartTime = today.AddHours(16), // 16:00
                            EndTime = today.AddHours(16).AddMinutes(kungFuPanda.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = kungFuPanda.Id,
                            CinemaRoomId = rooms[3].Id,
                            StartTime = today.AddHours(19), // 19:00
                            EndTime = today.AddHours(19).AddMinutes(kungFuPanda.Duration),
                            IsActive = true
                        },
                        new Showtime
                        {
                            MovieId = kungFuPanda.Id,
                            CinemaRoomId = rooms[0].Id,
                            StartTime = today.AddHours(21), // 21:00
                            EndTime = today.AddHours(21).AddMinutes(kungFuPanda.Duration),
                            IsActive = true
                        }
                    };

                    context.Showtimes.AddRange(kungFuShowtimes);
                }

                // Thêm showtime cho các phim khác (tuỳ chọn)
                if (avengers != null && rooms.Any())
                {
                    var today = DateTime.Today;
                    context.Showtimes.Add(new Showtime
                    {
                        MovieId = avengers.Id,
                        CinemaRoomId = rooms[1].Id,
                        StartTime = today.AddHours(15),
                        EndTime = today.AddHours(15).AddMinutes(avengers.Duration),
                        IsActive = true
                    });
                }

                if (dune != null && rooms.Any())
                {
                    var today = DateTime.Today;
                    context.Showtimes.Add(new Showtime
                    {
                        MovieId = dune.Id,
                        CinemaRoomId = rooms[2].Id,
                        StartTime = today.AddHours(20),
                        EndTime = today.AddHours(20).AddMinutes(dune.Duration),
                        IsActive = true
                    });
                }

                context.SaveChanges();
            }
        }
    }
}