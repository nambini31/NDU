using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Image
    {
        public long Id { get; set; }
        public long? BatchId { get; set; }
        public long? DocumentId { get; set; }
        public string? Side { get; set; }
        public int ScannedOrder { get; set; }
        public int Size { get; set; }
        public int Status { get; set; }
        public long? DocumentTypeId { get; set; }
        public byte[]? Data { get; set; }
        public string? Path { get; set; }
        public string? ImageToBase64String { get; set; }

        [ForeignKey("BatchId")]
        public  Batch? Batch { get; set; }

        [ForeignKey("DocumentId")]
        public  Document? Document { get; set; }

        [ForeignKey("DocumentTypeId")]
        public  DocumentType? DocumentType { get; set; }
    }
}
