using Core.Entity.Exorabilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO.Exorabilis
{
    public class DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
    {
        public long Id { get; set; }
        public long ImageId { get; set; }
        public long? BatchId { get; set; }
        public string IdEncrypt { get; set; }
        public long? FileId { get; set; }
        public string? BatchNumber { get; set; }
        public string? DocumentNumber { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? DocumentStatusName { get; set; }
        public string? DocumentStepName { get; set; }
        public string? RejectionCodeName { get; set; }
        public long? DocumentStatusId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }

        public int? PageOrder { get; set; }
        public int? GroupCount { get; set; }
        public int? pagesCount { get; set; }
        public int? ExportStatus { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string? Pcname { get; set; }
        public string? FileTypeName { get; set; }
        public long? FileTypeId { get; set; }
        public DateTime? IndexedOn { get; set; }
        public string? IndexedBy { get; set; }
        public DateTime? ExportOn { get; set; }
        public DateTime? CroppedOn { get; set; }
        public string? CroppedBy { get; set; }
        public DateTime? SanityOn { get; set; }
        public string? SanityBy { get; set; }
        public string? LastStep { get; set; }
        public DateTime? FinalQualityOn { get; set; }
        public string? FinalQualityBy { get; set; }
        public string? ReasonOther { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string? ReviewedBy { get; set; }

        public DateTime? RescanOn { get; set; }
        public string? RescanBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string? RejectedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? QualityOn { get; set; }
        public string? QualityBy { get; set; }
        public string? LicenseNumber { get; set; }

        public IEnumerable<DocumentIndex>? IndexModels { get; set; }
    }
}
