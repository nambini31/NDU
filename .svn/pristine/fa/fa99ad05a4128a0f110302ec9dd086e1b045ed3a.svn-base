using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class DocumentHistory
    {
        public long Id { get; set; }
        public long? BatchId { get; set; }
        public long? DocumentId { get; set; }
        public long? DocumentStepId { get; set; }
        public long? DocumentStatusId { get; set; }
        public long? RejectionCodeId { get; set; }
        public long? Amount { get; set; }
        public string? Mirc { get; set; }
        public long? WorkStationId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }

        [ForeignKey("BatchId")]
        public Batch? Batch { get; set; }

        [ForeignKey("DocumentId")]
        public  Document? Document { get; set; }

        [ForeignKey("DocumentStatusId")]
        public DocumentStatus? DocumentStatus { get; set; }

        [ForeignKey("RejectionCodeId")]
        public RejectionCode? RejectionCode { get; set; }
    }
}
