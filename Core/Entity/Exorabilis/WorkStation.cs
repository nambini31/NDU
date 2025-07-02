using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class WorkStation
    {

        public long Id { get; set; }
        public string? MacAddress { get; set; }
        public string? Reference { get; set; }

        public virtual ICollection<BatchHistory>? BatchHistory { get; set; }
    }
}
