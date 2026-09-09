using Bookstore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Api.Data;

public class BookstoreDbContext : DbContext
{
    public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(e =>
        {
            e.ToTable("Author", schema: "Authors");
            e.HasKey(a => a.AuthorId);
            e.Property(a => a.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(e =>
        {
            e.ToTable("Book", schema: "Books");
            e.HasKey(b => b.BookId);
            e.Property(b => b.Title).IsRequired().HasMaxLength(100);
            e.Property(b => b.SubTitle).HasMaxLength(200);
            e.HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId);
        });
    }
}
