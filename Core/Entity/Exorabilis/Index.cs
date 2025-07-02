using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Index
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? DataTypeId { get; set; }
        public long? Order { get; set; }
        public int? Status { get; set; }
        public int? IsExport { get; set; }
        public int? IsFileName { get; set; }
        public int? IsRequired { get; set; }
        public int? IsUnique { get; set; }


        [ForeignKey("DataTypeId")]
        public IndexDataType? IndexDataType { get; set; }
        public ICollection<DocumentIndex>? DocumentIndex { get; set; }
    }
}
