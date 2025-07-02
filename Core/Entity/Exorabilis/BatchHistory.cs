using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class BatchHistory
    {
        public long Id { get; set; }
        public long? BatchId { get; set; }
        public long? DocumentStatusId { get; set; }
        public long? DocumentStepId { get; set; }
        public long? RejectionCode { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? EndedOn { get; set; }
        public int? NumberOfDocument { get; set; }
        public string? UserId { get; set; }
        public long? WorkStationId { get; set; }
        public string? Remark { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string? FileNumber { get; set; }

        [ForeignKey("BatchId")]
        public  Batch? Batch { get; set; }

        [ForeignKey("DocumentStatusId")]
        public DocumentStatus? DocumentStatus { get; set; }

        [ForeignKey("RejectionCode")]
        public RejectionCode? RejectionCodeObjet { get; set; }
        
        [ForeignKey("WorkStationId")]
        public WorkStation? WorkStation { get; set; }

    }
}
