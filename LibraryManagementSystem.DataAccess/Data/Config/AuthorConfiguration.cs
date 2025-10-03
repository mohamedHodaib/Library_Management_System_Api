using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Data.Config
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Biography)
                 .IsRequired()
                 .HasMaxLength(1000);


            builder.HasOne(x => x.Person)
                   .WithOne(x => x.Author)
                   .HasForeignKey<Author>(a => a.PersonId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            //Seeding Data
            builder.HasData(
                new Author
                {
                    Id = 1,
                    PersonId = 1, // Corresponds to George Orwell's Person Id
                    Biography = "Eric Arthur Blair, known by his pen name George Orwell" +
                    ", was an English novelist, essayist, journalist and critic. His work " +
                    "is characterised by lucid prose, biting social criticism, opposition to totalitarianism, " +
                    "and outspoken support of democratic socialism."
                },
                new Author
                {
                    Id = 2,
                    PersonId = 2, // Corresponds to Jane Austen's Person Id
                    Biography = "Jane Austen was an English novelist known primarily for her six" +
                    " major novels, which interpret, critique and comment upon the British landed " +
                    "gentry at the end of the 18th century. Austen's plots often explore the dependence" +
                    " of women on marriage for social standing and economic security."
                },
                new Author
                {
                    Id = 3,
                    PersonId = 3, // Corresponds to Mark Twain's Person Id
                    Biography = "Samuel Langhorne Clemens, known by his pen name Mark Twain," +
                    " was an American writer, humorist, entrepreneur, publisher, and lecturer." +
                    " He was lauded as the 'greatest humorist the United States has produced', " +
                    "and William Faulkner called him 'the father of American literature'."
                }
            );

            builder.ToTable("Authors");
        }
    }
}
