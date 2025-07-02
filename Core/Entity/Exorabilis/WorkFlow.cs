using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class WorkFlow
    {

        public string? Id { get; set; }
        public long FromStepId { get; set; }
        public long ToStepId { get; set; }
        public int Status { get; set; }

    }
}
