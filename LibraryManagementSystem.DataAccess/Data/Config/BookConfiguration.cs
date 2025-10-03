using LibraryManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.DataAccess.Data.Config
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ISBN)
                .HasMaxLength(13)
                .IsRequired();

            builder.HasIndex(x => x.ISBN).IsUnique();

            builder.Property(x => x.PublishYear)
                .HasMaxLength(100);



            builder.HasMany(x => x.Authors)
                   .WithMany(x => x.Books)
                   .UsingEntity(x => x.ToTable("BookAuthors"));


            builder.HasMany(x => x.Borrowers)
                   .WithMany(x => x.Books)
                   .UsingEntity<Borrowing>();


            //Ingnore Concluded properties
            builder.Ignore(x => x.IsDeleted);

            //Seeding Data
            builder.HasData(
                 new Book
                 {
                     Id = 1,
                     Title = "Nineteen Eighty-Four",
                     ISBN = "9780451524935",
                     PublishYear = 1949,
                     DeleteDate = null
                 },
                 new Book
                 {
                     Id = 2,
                     Title = "Animal Farm",
                     ISBN = "9780451526342",
                     PublishYear = 1945,
                     DeleteDate = null
                 },
                 new Book
                 {
                     Id = 3,
                     Title = "Pride and Prejudice",
                     ISBN = "9780141439518",
                     PublishYear = 1813,
                     DeleteDate = null
                 },
                 new Book
                 {
                     Id = 4,
                     Title = "The Adventures of Tom Sawyer",
                     ISBN = "9780486400778",
                     PublishYear = 1876,
                     DeleteDate = null
                 }
            );

            builder.ToTable("Books");

        }
    }
}
