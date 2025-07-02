using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entity.Exorabilis
{
    public class Document
    {
        public long Id { get; set; }
        public long? BatchId { get; set; }
        public long? FileId { get; set; }
        public string? ReferenceNumber { get; set; }
        public long? DocumentTypeId { get; set; }
        public int? PageOrder { get; set; }
        public long? DocumentStepId { get; set; }
        public long? RejectionCodeId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? QualityOn { get; set; }
        public string? QualityBy { get; set; }
        public DateTime? IndexedOn { get; set; }
        public string? IndexedBy { get; set; }
        public DateTime? CroppedOn { get; set; }
        public string? CroppedBy { get; set; }
        public DateTime? SanityOn { get; set; }
        public string? SanityBy { get; set; }
        public DateTime? RescanOn { get; set; }
        public string? RescanBy { get; set; }
        public string? Pcname { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? FinalQualityOn { get; set; }
        public string? FinalQualityBy { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public DateTime? ExportOn { get; set; }
        public string? ReviewedBy { get; set; }
        public string? LastStep { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ReasonOther { get; set; }
        public long? DocumentStatusId { get; set; }
        public int? ExportStatus { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }

        //public  Batch? Batch { get; set; }

        [ForeignKey("DocumentStatusId")]
        public  DocumentStatus? DocumentStatus { get; set; }

        [ForeignKey("DocumentStepId")]
        public  DocumentStep? DocumentStep { get; set; }

        [ForeignKey("DocumentTypeId")]
        public  DocumentType? DocumentType { get; set; }

        [ForeignKey("FileId")]
        public Files? File { get; set; }

        [ForeignKey("RejectionCodeId")]
        public  RejectionCode? RejectionCode { get; set; }
        public  ICollection<DocumentHistory>? DocumentHistory { get; set; }
        public  ICollection<DocumentIndex>? DocumentIndex { get; set; }
        public  ICollection<Image>? Image { get; set; }
    }

    public class DocumentQualityControl
    {
        public long Id { get; set; }
        public long? FileId { get; set; }
        public string BatchNumber { get; set; }
        public long? Type { get; set; }
        public string DocumentNumber { get; set; }
        public long? DocumentStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? DocumentStepId { get; set; }
        public int? PageOrder { get; set; }
        public long? RejectionCodeId { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string ImageContentStr { get; set; }
        public string ImageVersoContentStr { get; set; }
        public string PathRecto { get; set; }
        public string PathVerso { get; set; }
        public string PathCropped { get; set; }
        public long? FileTypeId { get; set; }

        public DateTime IndexedOn { get; set; }
        public string IndexedBy { get; set; }
        public DateTime CroppedOn { get; set; }
        public string CroppedBy { get; set; }
        public DateTime SanityOn { get; set; }
        public string SanityBy { get; set; }
        public DateTime FinalQualityOn { get; set; }
        public string FinalQualityBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }

        public string LicenseNumber { get; set; }
    }

    public class DocumentReview
    {
        public long Id { get; set; }
        public long? FileId { get; set; }
        public string BatchNumber { get; set; }
        public string DocumentNumber { get; set; }
        public long? Type { get; set; }
        public long? DocumentStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? DocumentStepId { get; set; }
        public int? PageOrder { get; set; }
        public long? RejectionCodeId { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string ImageContentStr { get; set; }
        public string ImageVersoContentStr { get; set; }
        public string PathRecto { get; set; }
        public string PathVerso { get; set; }
        public string PathCropped { get; set; }
        public long FileTypeId { get; set; }
        public string LicenseNumber { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
        public string NIC { get; set; }
        public DateTime IndexedOn { get; set; }
        public string IndexedBy { get; set; }
        public DateTime CroppedOn { get; set; }
        public string CroppedBy { get; set; }
        public DateTime SanityOn { get; set; }
        public string SanityBy { get; set; }
        public DateTime FinalQualityOn { get; set; }
        public string FinalQualityBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }
        public string Comment { get; set; }
        public long? DocumentHistoryId { get; set; }
    }

    public class DocumentQualityControlCount
    {
        public DocumentQualityControlCount()
        {
        }
        public long? Id { get; set; }
        public long Count { get; set; }
    }


    public class DashboardListDate
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public partial class DashboardCount
    {
        public long Id { get; set; }
        public long ScannedCount { get; set; }
        public long QualityCount { get; set; }

        public long IndexedCount { get; set; }
        public int CroppedCount { get; set; }
        public int SanityCount { get; set; }

        public int FinalQualityCount { get; set; }


        public long DocumentScanned { get; set; }
        public long DocumentQuality { get; set; }
        public long DocumentIndexed { get; set; }
        public long DocumentSanity { get; set; }
        public long DocumentCropped { get; set; }
        public long DocumentFinal { get; set; }
        public long AllDocumentScanned { get; set; }
        public long AllDocumentQuality { get; set; }
        public long AllDocumentIndexed { get; set; }
        public long AllDocumentSanity { get; set; }
        public long AllDocumentCropped { get; set; }
        public long AllDocumentFinal { get; set; }
    }

    public class DocumentSearch
    {
        public DocumentSearch()
        {
            IndexedOn = default;
            CroppedOn = default;
            SanityOn = default;
            FinalQualityOn = default;
            ReviewedOn = default;
        }
        public long Id { get; set; }
        public long? FileId { get; set; }
        public string BatchNumber { get; set; }
        public string DocumentNumber { get; set; }
        public long? Type { get; set; }
        public long? DocumentStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? DocumentStepId { get; set; }
        public int? PageOrder { get; set; }
        public long? RejectionCodeId { get; set; }
        public DateTimeOffset? LastUpdatedOn { get; set; }
        public string ImageContentStr { get; set; }
        public string ImageVersoContentStr { get; set; }
        public string PathRecto { get; set; }
        public string PathVerso { get; set; }
        public string PathCropped { get; set; }
        public string LicenseNumber { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
        public string NIC { get; set; }
        public long FileTypeId { get; set; }
        public DateTime IndexedOn { get; set; }
        public string IndexedBy { get; set; }
        public DateTime CroppedOn { get; set; }
        public string CroppedBy { get; set; }
        public DateTime SanityOn { get; set; }
        public string SanityBy { get; set; }
        public DateTime FinalQualityOn { get; set; }
        public string FinalQualityBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }
    }


    public partial class UserDocumentValidation
    {
        public int Id { get; set; }
    }


    public partial class ClearTable
    {
        public ClearTable()
        {
        }
        public int Id { get; set; }
        public long Response { get; set; }

    }
}
