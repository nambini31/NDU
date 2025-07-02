using System.Collections.Generic;

namespace Core.Entity.MyCore
{
    public class Person
    {
        public Person()
        {
            User = new HashSet<User>();
        }

        public string Id { get; set; }
        public long MyId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }
        public string? Gender { get; set; }
        public string? Title { get; set; }
        public string? JobTitle { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
