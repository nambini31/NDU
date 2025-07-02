using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class DocumentStep
    {

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }

        public ICollection<Batch>? Batch { get; set; }
        public ICollection<Document>? Document { get; set; }

    }
}
