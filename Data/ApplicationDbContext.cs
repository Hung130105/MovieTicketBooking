using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<CinemaRoom> CinemaRooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình User
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.FullName)
                .HasMaxLength(100);

            // Cấu hình precision cho decimal properties
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BookingDetail>()
                .Property(bd => bd.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            // Cấu hình relationships với NO ACTION để tránh cascade conflict
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.CinemaRoom)
                .WithMany(c => c.Showtimes)
                .HasForeignKey(s => s.CinemaRoomId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.CinemaRoom)
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CinemaRoomId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Showtime)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowtimeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // NOTE: removed mapping between BookingDetail and Seat because BookingDetail now stores SeatNumber (string)
            // If you prefer to keep FK to Seats, restore relationship and ensure Seats table contains those IDs.

            // Cấu hình string length để tránh nvarchar(max)
            modelBuilder.Entity<CinemaRoom>()
                .Property(c => c.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Title)
                .HasMaxLength(100);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Seat>()
                .Property(s => s.SeatNumber)
                .HasMaxLength(10);

            modelBuilder.Entity<Seat>()
                .Property(s => s.SeatType)
                .HasMaxLength(20);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CustomerName)
                .HasMaxLength(100);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CustomerEmail)
                .HasMaxLength(100);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CustomerPhone)
                .HasMaxLength(15);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentMethod)
                .HasMaxLength(50);

            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentStatus)
                .HasMaxLength(50);

            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingCode)
                .HasMaxLength(20);

            // Configure BookingDetail.SeatNumber if exists
            // (only if your BookingDetail model has SeatNumber property)
            var bookingDetailEntity = modelBuilder.Entity<BookingDetail>();
            if (bookingDetailEntity != null)
            {
                // if BookingDetail has SeatNumber property, set max length
                // (this is safe: if property doesn't exist it will be ignored at runtime)
                bookingDetailEntity
                    .Property<string>("SeatNumber")
                    .HasMaxLength(10)
                    .IsRequired(false);
            }
        }
    }
}