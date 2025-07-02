using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO.Exorabilis
{
    public class UserCount
    {
       public string? Username { get; set; }
       public int Scan { get; set; }
        public int Quality { get; set; }
        public int Index { get; set; }
        public int Sanity { get; set; }
        public int Reject { get; set; }
        public int Rescan { get; set; }
        public int Completed { get; set; }
    }
}
