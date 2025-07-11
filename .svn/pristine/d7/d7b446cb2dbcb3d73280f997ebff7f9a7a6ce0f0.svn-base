using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Batch
    {

        public long Id { get; set; }
        public string? ReferenceNumber { get; set; }
        public int? NumberOfDocument { get; set; }
        public DateTime? ScannedOn { get; set; }
        public long? DocumentTypeId { get; set; }
        public long? DocumentStatusId { get; set; }
        public long? DocumentStepId { get; set; }
        public long? RejectionCodeId { get; set; }
        public int? IsLocked { get; set; }
        public string? LockedBy { get; set; }

        public DateTime? QualityOn { get; set; }
        public DateTime? ExportOn { get; set; }
        public string? QualityBy { get; set; }
        public DateTime? IndexedOn { get; set; }
        public string? IndexedBy { get; set; }
        public DateTime? SanityOn { get; set; }
        public string? SanityBy { get; set; }
        public DateTime? RescanOn { get; set; }
        public string? RescanBy { get; set; }
        public string? ScannedBy { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string? ReviewedBy { get; set; }
        public string? LastStep { get; set; }
        public DateTime? LockedOn { get; set; }
        public string? ReasonOther { get; set; }
        public string? Pcname { get; set; }
        public string? FileNumber { get; set; }

        public int? ExportStatus { get; set; }
        public DateTime? LastUpdatedOn { get; set; }

        [ForeignKey("DocumentStatusId")]
        public DocumentStatus? DocumentStatus { get; set; }

        [ForeignKey("DocumentStepId")]
        public DocumentStep? DocumentStep { get; set; }

        [ForeignKey("DocumentTypeId")]
        public DocumentType? DocumentType { get; set; }


        [ForeignKey("RejectionCodeId")]
        public RejectionCode? RejectionCode { get; set; }

        public ICollection<Image>? Image{ get; set; }
        public Files? File{ get; set; }

        public ICollection<BatchHistory>? BatchHistory { get; set; }

    }
}
