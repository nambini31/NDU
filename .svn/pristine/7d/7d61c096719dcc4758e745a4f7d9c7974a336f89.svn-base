using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Session
    {
        public long Id { get; set; }
        public long? CycleId { get; set; }
        public long? SessionStateId { get; set; }
        public DateTime? OpenedOn { get; set; }
        public string? OpenedBy { get; set; }
        public DateTime? ClosedOn { get; set; }
        public string? ClosedBy { get; set; }

        [ForeignKey("CycleId")]
        public Cycle? Cycle { get; set; }

        [ForeignKey("SessionStateId")]
        public SessionState? SessionState { get; set; }
    }
}
