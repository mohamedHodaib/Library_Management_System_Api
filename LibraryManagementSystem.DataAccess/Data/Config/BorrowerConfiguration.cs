using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Data.Config
{
    public class BorrowerConfiguration : IEntityTypeConfiguration<Borrower>
    {
        public void Configure(EntityTypeBuilder<Borrower> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Person)
                   .WithOne(x => x.Borrower)
                   .HasForeignKey<Borrower>(a => a.PersonId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);


            //Seeding Data
            builder.HasData(
                new Borrower
                {
                    Id = 1,
                    PersonId = 4 // Corresponds to John Smith's Person Id
                },
                new Borrower
                {
                    Id = 2,
                    PersonId = 5 // Corresponds to Emily Jones's Person Id
                }
            );

            builder.ToTable("Borrowers");
        }
    }
}
