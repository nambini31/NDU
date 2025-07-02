using System;

namespace Core.Entity.MyCore
{
    public class AccountRecovery
    {
        public string Id { get; set; }
        public string Otp { get; set; }
        public string Email { get; set; }
        public string PersonId { get; set; }
        public int Used { get; set; }

        public DateTime Expirytime { get; set; }
    }
}
