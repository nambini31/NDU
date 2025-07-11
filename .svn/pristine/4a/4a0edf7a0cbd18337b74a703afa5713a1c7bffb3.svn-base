using System;

namespace Core.Entity.MyCore
{
    public class Notification
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Link { get; set; }
        public string Message { get; set; }
        public string Receiver { get; set; }
        public string Type { get; set; }
        public int? IsRead { get; set; }
        public DateTime Date { get; set; }

        public virtual User ReceiverNavigation { get; set; }
    }
}