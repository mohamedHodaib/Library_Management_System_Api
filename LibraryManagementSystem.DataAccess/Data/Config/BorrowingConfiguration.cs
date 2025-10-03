using LibraryManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Data.Config
{
    internal class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BorrowDate)
                .HasColumnType("Date")
                .IsRequired();

            builder.Property(x => x.ReturnDate)
                .HasColumnType("Date");

            //Ingnore Concluded properties
            builder.Ignore(x => x.IsReturned);
            builder.Ignore(x => x.IsDeleted);


            //Seeding Data
            builder.HasData(
                 new Borrowing
                 {
                     Id = 1,
                     BookId = 1, // Nineteen Eighty-Four
                     BorrowerId = 1, // John Smith
                     BorrowDate = new DateOnly(2025, 9, 1),
                     ReturnDate = null, // Not yet returned
                     DeleteDate = null
                 },
                new Borrowing
                {
                    Id = 2,
                    BookId = 3, // Pride and Prejudice
                    BorrowerId = 1, // John Smith
                    BorrowDate = new DateOnly(2025, 8, 15),
                    ReturnDate = new DateOnly(2025, 8, 29), // Returned
                    DeleteDate = null
                },
                new Borrowing
                {
                    Id = 3,
                    BookId = 2, // Animal Farm
                    BorrowerId = 2, // Emily Jones
                    BorrowDate = new DateOnly(2025, 9, 10),
                    ReturnDate = null, // Not yet returned
                    DeleteDate = null
                }
            );


        }
    }
}
