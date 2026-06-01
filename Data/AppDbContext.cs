
using Microsoft.EntityFrameworkCore;
using MrLMS.Models;

namespace MrLMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberRole> MemberRoles { get; set; }
        public DbSet<MemberSubscription> MemberSubscriptions { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<BookReservation> BookReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book → Genre
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.SetNull);

            // BookAuthor → Book
            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookAuthor → Author
            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // MemberRole → Member
            modelBuilder.Entity<MemberRole>()
                .HasOne(mr => mr.Member)
                .WithMany(m => m.MemberRoles)
                .HasForeignKey(mr => mr.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // MemberRole → Role
            modelBuilder.Entity<MemberRole>()
                .HasOne(mr => mr.Role)
                .WithMany(r => r.MemberRoles)
                .HasForeignKey(mr => mr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // MemberSubscription → Member
            modelBuilder.Entity<MemberSubscription>()
                .HasOne(ms => ms.Member)
                .WithMany(m => m.MemberSubscriptions)
                .HasForeignKey(ms => ms.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // MemberSubscription → Subscription
            modelBuilder.Entity<MemberSubscription>()
                .HasOne(ms => ms.Subscription)
                .WithMany(s => s.MemberSubscriptions)
                .HasForeignKey(ms => ms.SubId)
                .OnDelete(DeleteBehavior.SetNull);

            // BookReview → Member
            modelBuilder.Entity<BookReview>()
                .HasOne(br => br.Member)
                .WithMany(m => m.BookReviews)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookReview → Book
            modelBuilder.Entity<BookReview>()
                .HasOne(br => br.Book)
                .WithMany(b => b.BookReviews)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookReservation → Member
            modelBuilder.Entity<BookReservation>()
                .HasOne(br => br.Member)
                .WithMany(m => m.BookReservations)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookReservation → Book
            modelBuilder.Entity<BookReservation>()
                .HasOne(br => br.Book)
                .WithMany(b => b.BookReservations)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index on Book.ISBN
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // Unique index on Member.Email
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();
        }
    }
}