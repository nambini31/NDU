using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Files
    {

        public long Id { get; set; }
        public int? IsFramework { get; set; }
        public long? Project_folio_id { get; set; }
        public long? BatchId { get; set; }
        public long? FileTypeId { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? Lastupdate { get; set; }

        [ForeignKey("FileTypeId")]
        public FileType? FileType { get; set; }
        public virtual ICollection<Document>? Document { get; set; }
    }
}
