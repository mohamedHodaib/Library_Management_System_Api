using LibraryManagementSystem.DataAccess.Entities;
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
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.Id);



            // Seed the owner Person entities
            builder.HasData(
                new  { Id = 1 },
                new  { Id = 2 },
                new  { Id = 3 },
                new  { Id = 4 },
                new  { Id = 5 },
                new  { Id = 6 },
                new  { Id = 7 }
            );

            // Configure and seed the owned PersonName entity
            builder.OwnsOne(x => x.Name, nameBuilder =>
            {
                nameBuilder.Property(x => x.FirstName).HasColumnName("FirstName").HasMaxLength(100);
                nameBuilder.Property(x => x.LastName).HasColumnName("LastName").HasMaxLength(100);

                // Seed data for the owned type, including the foreign key
                nameBuilder.HasData(
                    new { PersonId = 1, FirstName = "George", LastName = "Orwell" },
                    new { PersonId = 2, FirstName = "Jane", LastName = "Austen" },
                    new { PersonId = 3, FirstName = "Mark", LastName = "Twain" },
                    new { PersonId = 4, FirstName = "John", LastName = "Smith" },
                    new { PersonId = 5, FirstName = "Emily", LastName = "Jones" },
                    new { PersonId = 6, FirstName = "Mohamed", LastName = "Hegazi" },
                    new { PersonId = 7, FirstName = "Ali", LastName = "Mohamed" }
                );
            });

        }
    }
}
