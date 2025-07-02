using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO.Exorabilis
{
    public class DocumentIndex_DTO
    {
        public long Id { get; set; }
        public string? Project_type_name { get; set; }
        public string? Project_reference { get; set; }
        public long? NbrOfPages { get; set; }
        public string? Folios { get; set; }
        public long? FileValueId { get; set; }
        public long? Project_type_value_id { get; set; }

    }
}
