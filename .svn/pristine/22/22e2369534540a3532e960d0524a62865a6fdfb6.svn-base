using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class DocumentType
    {

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Batch>? Batch { get; set; }
        public virtual ICollection<Document>? Document { get; set; }
        public virtual ICollection<Image>? Image { get; set; }
    }
}
