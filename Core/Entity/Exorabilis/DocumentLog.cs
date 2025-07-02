using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class DocumentLog
    {
        public long Id { get; set; }
        public string? DeliveredBy { get; set; }
        public string? ReceivedBy { get; set; }
        public string? Log { get; set; }
    }
}
