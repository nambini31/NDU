using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class DocumentStatus
    {

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }

        public ICollection<Batch>? Batch { get; set; }
        public  ICollection<BatchHistory>? BatchHistory { get; set; }
        public  ICollection<Document>? Document { get; set; }
        public  ICollection<DocumentHistory>? DocumentHistory { get; set; }
    }
}
