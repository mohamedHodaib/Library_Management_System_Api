namespace LibraryManagementSystem.DataAccess.Entities.People
{
    public class PersonName
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
