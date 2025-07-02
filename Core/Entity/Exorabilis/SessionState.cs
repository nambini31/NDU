using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class SessionState
    {

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Session> Session { get; set; }
    }
}
