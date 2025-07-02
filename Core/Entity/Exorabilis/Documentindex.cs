using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class DocumentIndex
    {
        public long Id { get; set; }
        public long? DocumentId { get; set; }
        public long? IndexId { get; set; }
        public string? Value { get; set; }
        public string?  OldValue { get; set; }
        public int? Status { get; set; }

        [ForeignKey("DocumentId")]
        public Document? Document { get; set; }
        
        [ForeignKey("IndexId")]
        public Index? Index { get; set; }
    }
}
