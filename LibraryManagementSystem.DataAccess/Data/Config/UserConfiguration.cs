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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
           builder
           .HasOne(user => user.Person)
           .WithOne(person => person.User)
           .HasForeignKey<User>(user => user.PersonId)
           .OnDelete(DeleteBehavior.Restrict); // Prevents the database from cascading to use the identity deleting
        }
    }
}
