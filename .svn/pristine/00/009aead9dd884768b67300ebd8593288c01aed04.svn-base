using System;
using System.Collections.Generic;

namespace Core.Entity.MyCore
{
    public class User
    {
        public User()
        {
            Session = new HashSet<Session>();
            UserRole = new HashSet<UserRole>();
            Notification = new HashSet<Notification>();
        }

        public string Id { get; set; }
        public long MyId { get; set; }
        public string PersonId { get; set; }
        public string UserName { get; set; }
        public string HashCode { get; set; }
        public string UserStatusId { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }
        public string? Mail { get; set; }
        public string? PasswordResetRequest { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual Person Person { get; set; }
        public virtual UserStatus UserStatus { get; set; }
        public virtual ICollection<Session> Session { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
    }
}
