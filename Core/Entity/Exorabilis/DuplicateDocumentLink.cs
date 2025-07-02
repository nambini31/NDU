using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class DuplicateDocumentLink
    {
        public long Id { get; set; }
        public long? DocId { get; set; }
        public long? LinkDocId { get; set; }
    }
}
