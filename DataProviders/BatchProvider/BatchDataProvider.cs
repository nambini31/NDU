using Core.Database;
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.GlobalVariables;
using Core.RequestPostGet;
using GdPicture.Internal.MSOfficeBinary.translator.Spreadsheet.XlsFileFormat.Records;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharp.Drawing;
using PdfDocument = PdfSharp.Pdf.PdfDocument;


namespace DataProviders.BatchProvider
{
    public class BatchDataProvider : InterfaceBatchDataProvider
    {
        private readonly ExorabilisContext exorabilisContext;
        public BatchDataProvider(ExorabilisContext exorabilis)
        {
            this.exorabilisContext = exorabilis;
        }

        /// <summary>
        ///  Affichage par batch en fonction de step ( 2 : quality , 3 : indexing , 5 : sanity , 8 : review  , 11 : batch to unlock , 12 : batch to rescan ) 
        /// </summary> si  step == 12  on recupere le status = 5 et 6 (completed to rescan)
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Batch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllBatchByStep(BatchSearchRequest request)
        {

            try
            {
                var doctypeid = Convert.ToInt64(GetValueParam("DefaultDocType").Value);


                var data = from b in exorabilisContext.Batch
                           join status in exorabilisContext.DocumentStatus on b.DocumentStatusId equals status.Id
                           join step in exorabilisContext.DocumentStep on b.DocumentStepId equals step.Id
                           join type in exorabilisContext.DocumentType on b.DocumentStatusId equals type.Id
                           join reject in exorabilisContext.RejectionCode on b.RejectionCodeId equals reject.Id into rejected
                           from reject in rejected.DefaultIfEmpty()
                           join file in exorabilisContext.Files on b.Id equals file.BatchId // assuming you want to join with File for the FileType
                           select new { b, status, step, type, file, reject };

                if (request.firstdate != null)
                {
                    if (request.step == 2) // quality
                    {
                        data = from d in data
                               where d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) >= request.firstdate
                               select d;
                    }
                    else if (request.step == 3) //  batch indexing
                    {
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) >= request.firstdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) >= request.firstdate)
                               select d;
                    }
                    else if (request.step == 5) //  batch sanity
                    {
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) >= request.firstdate) || (d.b.IndexedOn.HasValue && DateOnly.FromDateTime(d.b.IndexedOn.Value) >= request.firstdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) >= request.firstdate)
                               select d;
                    }
                    else if (request.step == 14) //  batch search
                    {
                        // si on passe par saim review
                        //data = from d in data
                        //       where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) >= request.firstdate)
                        //       select d;
                        //pour EUIPO
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) >= request.firstdate)
                               select d;
                    }
                    else if (request.step == 11)// UNLOCK BAtch 11
                    {
                        data = from d in data
                               where (d.b.LockedOn.HasValue && DateOnly.FromDateTime(d.b.LockedOn.Value) >= request.firstdate) || (d.b.SanityOn.HasValue && DateOnly.FromDateTime(d.b.SanityOn.Value) >= request.firstdate) || (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) >= request.firstdate) || (d.b.IndexedOn.HasValue && DateOnly.FromDateTime(d.b.IndexedOn.Value) >= request.firstdate) || (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) >= request.firstdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) >= request.firstdate)
                               select d;
                    }
                    else  // batch to rescan 12
                    {

                        data = from d in data
                               where (d.b.RescanOn.HasValue && DateOnly.FromDateTime(d.b.RescanOn.Value) >= request.firstdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) >= request.firstdate)
                               select d;

                    }


                }

                if (request.lastdate != null)
                {
                    if (request.step == 2) // quality
                    {
                        data = from d in data
                               where d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) <= request.lastdate
                               select d;
                    }
                    else if (request.step == 3) //  batch indexing
                    {
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) <= request.lastdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) <= request.lastdate)
                               select d;
                    }
                    else if (request.step == 5) //  batch sanity
                    {
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) <= request.lastdate) || (d.b.IndexedOn.HasValue && DateOnly.FromDateTime(d.b.IndexedOn.Value) <= request.lastdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) <= request.lastdate)
                               select d;
                    }
                    else if (request.step == 14) //  batch search
                    {
                        // si on passe par saim review
                        //data = from d in data
                        //       where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) <= request.lastdate)
                        //       select d;

                        // pour EUIPO
                        data = from d in data
                               where (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) <= request.lastdate)
                               select d;
                    }
                    else if (request.step == 11)// UNLOCK BAtch 11
                    {
                        data = from d in data
                               where (d.b.LockedOn.HasValue && DateOnly.FromDateTime(d.b.LockedOn.Value) <= request.lastdate) || (d.b.SanityOn.HasValue && DateOnly.FromDateTime(d.b.SanityOn.Value) <= request.lastdate) || (d.b.QualityOn.HasValue && DateOnly.FromDateTime(d.b.QualityOn.Value) <= request.lastdate) || (d.b.IndexedOn.HasValue && DateOnly.FromDateTime(d.b.IndexedOn.Value) <= request.lastdate) || (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) <= request.lastdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) <= request.lastdate)
                               select d;
                    }
                    else  // batch to rescan 12
                    {

                        data = from d in data
                               where (d.b.RescanOn.HasValue && DateOnly.FromDateTime(d.b.RescanOn.Value) <= request.lastdate) || (d.b.ScannedOn.HasValue && DateOnly.FromDateTime(d.b.ScannedOn.Value) <= request.lastdate)
                               select d;

                    }
                }

                if (request.batchnumber != null)
                {
                    data = from d in data
                           where d.b.ReferenceNumber.Contains(request.batchnumber)
                           select d;
                }


                if (request.step != 11 && request.step != 12 && request.step != 14)
                {
                    var result = (from d in data
                                  where d.b.DocumentStepId == request.step
                               && d.b.DocumentStatusId == 4
                               && !d.b.RejectionCodeId.HasValue
                               && d.b.NumberOfDocument > 0
                                  select new Batch_Quality_Crop_Index_Sanity_Unlock_DTO
                                  {
                                      BatchNumber = d.b.ReferenceNumber,
                                      NumberOfDocument = (from img in exorabilisContext.Image
                                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                          where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId != 6)
                                                          select docx).Count() / 2,
                                      NumberOfPages = (from img in exorabilisContext.Image
                                                       join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                       where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId != 6)
                                                       select docx).Count(),
                                      StatusName = d.status.Name,
                                      StepName = d.step.Name,
                                      ExportStatus = d.b.ExportStatus,

                                      LastStep = d.b.LastStep,

                                      QualityBy = d.b.QualityBy,
                                      QualityOn = d.b.QualityOn,
                                      Pcname = d.b.Pcname,

                                      RescanBy = d.b.RescanBy,
                                      RescanOn = d.b.RescanOn,

                                      IndexedOn = d.b.IndexedOn,
                                      LockedOn = d.b.LockedOn,
                                      IndexedBy = d.b.IndexedBy,

                                      SanityOn = d.b.SanityOn,
                                      SanityBy = d.b.SanityBy,
                                      FileNumber = d.b.FileNumber,
                                      ReviewedOn = d.b.ReviewedOn,
                                      ReviewedBy = d.b.ReviewedBy,
                                      RejectName = d.reject.Name,
                                      StatusId = d.status.Id,
                                      ReasonOther = d.b.ReasonOther,
                                      TypeName = d.type.Name,
                                      ScannedOn = d.b.ScannedOn,
                                      ScannedBy = d.b.ScannedBy,
                                      IsLocked = d.b.IsLocked,
                                      IdEncrypt = d.b.Id.ToString(),
                                      LockedBy = d.b.LockedBy ?? "",
                                      FileTypeName = d.file.FileType.Name ?? "",
                                      FileTypeId = d.file.FileTypeId,
                                      Id = d.b.Id

                                  }).Distinct().ToList();
                    return result;
                }
                else if (request.step == 14) // search
                {
                    var result = (from d in data
                                  where d.b.DocumentStepId == 8
                               && d.b.DocumentStatusId == 1
                               && !d.b.RejectionCodeId.HasValue
                               && d.b.NumberOfDocument > 0
                                  select new Batch_Quality_Crop_Index_Sanity_Unlock_DTO
                                  {
                                      BatchNumber = d.b.ReferenceNumber,
                                      NumberOfDocument = (from img in exorabilisContext.Image
                                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                          where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId == 1)
                                                          select docx).Count() / 2,
                                      NumberOfPages = (from img in exorabilisContext.Image
                                                       join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                       where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId == 1)
                                                       select docx).Count(),
                                      StatusName = d.status.Name,
                                      StepName = d.step.Name,
                                      RejectName = d.reject.Name,
                                      StatusId = d.status.Id,
                                      ReasonOther = d.b.ReasonOther,
                                      TypeName = d.type.Name,
                                      ScannedOn = d.b.ScannedOn,
                                      ScannedBy = d.b.ScannedBy,

                                      ExportStatus = d.b.ExportStatus,
                                      LockedOn = d.b.LockedOn,
                                      LastStep = d.b.LastStep,
                                      FileNumber = d.b.FileNumber,

                                      QualityBy = d.b.QualityBy,
                                      QualityOn = d.b.QualityOn,
                                      Pcname = d.b.Pcname,

                                      RescanBy = d.b.RescanBy,
                                      RescanOn = d.b.RescanOn,

                                      IndexedOn = d.b.IndexedOn,
                                      IndexedBy = d.b.IndexedBy,

                                      SanityOn = d.b.SanityOn,
                                      SanityBy = d.b.SanityBy,

                                      ReviewedOn = d.b.ReviewedOn,
                                      ReviewedBy = d.b.ReviewedBy,

                                      IsLocked = d.b.IsLocked,
                                      IdEncrypt = d.b.Id.ToString(),
                                      LockedBy = d.b.LockedBy ?? "",
                                      FileTypeName = d.file.FileType.Name ?? "",
                                      FileTypeId = d.file.FileTypeId,
                                      Id = d.b.Id

                                  }).Distinct().ToList();
                    return result;
                }
                else if (request.step == 11)// UNLOCK BAtch 11
                {
                    var result = (from d in data
                                  where d.b.DocumentStepId != 1
                             && d.b.IsLocked == 1 &&
                             (d.b.DocumentStatusId != 5 && d.b.DocumentStatusId != 6)
                               && !d.b.RejectionCodeId.HasValue
                               && d.b.NumberOfDocument > 0
                                  select new Batch_Quality_Crop_Index_Sanity_Unlock_DTO
                                  {
                                      BatchNumber = d.b.ReferenceNumber,
                                      NumberOfDocument = (from img in exorabilisContext.Image
                                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                          where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId != 6)
                                                          select docx).Count() / 2,
                                      NumberOfPages = (from img in exorabilisContext.Image
                                                       join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                       where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1 && (docx.DocumentStatusId != 6)
                                                       select docx).Count(),
                                      StepName = d.step.Name,
                                      TypeName = d.type.Name,
                                      StatusId = d.status.Id,
                                      RejectName = d.reject.Name,
                                      ReasonOther = d.b.ReasonOther,
                                      ExportStatus = d.b.ExportStatus,
                                      LastStep = d.b.LastStep,
                                      LockedOn = d.b.LockedOn,
                                      QualityBy = d.b.QualityBy,
                                      QualityOn = d.b.QualityOn,
                                      ScannedBy = d.b.ScannedBy,
                                      Pcname = d.b.Pcname,
                                      FileNumber = d.b.FileNumber,

                                      RescanBy = d.b.RescanBy,
                                      RescanOn = d.b.RescanOn,

                                      IndexedOn = d.b.IndexedOn,
                                      IndexedBy = d.b.IndexedBy,

                                      SanityOn = d.b.SanityOn,
                                      SanityBy = d.b.SanityBy,

                                      ReviewedOn = d.b.ReviewedOn,
                                      ReviewedBy = d.b.ReviewedBy,
                                      ScannedOn = d.b.ScannedOn,
                                      IsLocked = d.b.IsLocked,
                                      IdEncrypt = d.b.Id.ToString(),
                                      LockedBy = d.b.LockedBy ?? "",
                                      FileTypeName = d.file.FileType.Name ?? "",
                                      FileTypeId = d.file.FileTypeId,
                                      Id = d.b.Id

                                  }).Distinct().ToList();
                    return result;
                }
                else  // batch to rescan 12
                {

                    var result = (from d in data
                                  where d.b.DocumentStepId != 1
                             && (d.b.DocumentStatusId == 5 || d.b.DocumentStatusId == 6)
                               && d.b.RejectionCodeId.HasValue
                                  select new Batch_Quality_Crop_Index_Sanity_Unlock_DTO
                                  {
                                      BatchNumber = d.b.ReferenceNumber,
                                      NumberOfDocument = (from img in exorabilisContext.Image
                                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                          where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1
                                                          select docx).Count() / 2,
                                      NumberOfPages = (from img in exorabilisContext.Image
                                                       join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                                       where docx.DocumentTypeId == doctypeid && img.BatchId == d.b.Id && img.Status == 1
                                                       select docx).Count(),
                                      StatusName = d.status.Name,
                                      StepName = d.step.Name,
                                      StatusId = d.status.Id,
                                      RejectName = d.reject.Name,
                                      ReasonOther = d.b.ReasonOther,
                                      ExportStatus = d.b.ExportStatus,
                                      LastStep = d.b.LastStep,
                                      LockedOn = d.b.LockedOn,
                                      QualityBy = d.b.QualityBy,
                                      QualityOn = d.b.QualityOn,
                                      FileNumber = d.b.FileNumber,

                                      RescanBy = d.b.RescanBy,
                                      RescanOn = d.b.RescanOn,
                                      ScannedBy = d.b.ScannedBy,

                                      IndexedOn = d.b.IndexedOn,
                                      IndexedBy = d.b.IndexedBy,
                                      Pcname = d.b.Pcname,

                                      SanityOn = d.b.SanityOn,
                                      SanityBy = d.b.SanityBy,

                                      ReviewedOn = d.b.ReviewedOn,
                                      ReviewedBy = d.b.ReviewedBy,
                                      TypeName = d.type.Name,
                                      ScannedOn = d.b.ScannedOn,
                                      IsLocked = d.b.IsLocked,
                                      IdEncrypt = d.b.Id.ToString(),
                                      LockedBy = d.b.LockedBy ?? "",
                                      FileTypeName = d.file.FileType.Name ?? "",
                                      FileTypeId = d.file.FileTypeId,
                                      Id = d.b.Id

                                  }).Distinct().ToList();
                    return result;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task excel_for_batch(BatchSearchRequest request)
        {


            try
            {
                var doctypeid = GetValueParam("DefaultDocType").Value;

                var doctype = new List<long> { Convert.ToInt64(doctypeid) };

                // to be qualited
                var scantotal = (from d in exorabilisContext.Document
                                 join f in exorabilisContext.Files on d.FileId equals f.Id
                                 join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                 where doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue
                                 select new { d, f });

                // to be qualited
                var scannedtotal = (from d in exorabilisContext.Document
                                    join f in exorabilisContext.Files on d.FileId equals f.Id
                                    join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                    where (d.DocumentStatusId != 5 && d.DocumentStatusId != 6 && d.DocumentStatusId != 2) && d.DocumentStepId == 2 && doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue
                                    select new { d, f });
                //TO BE indexing
                var qualitytotal = (from d in exorabilisContext.Document
                                    join f in exorabilisContext.Files on d.FileId equals f.Id
                                    join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                    where (d.DocumentStatusId != 5 && d.DocumentStatusId != 6 && d.DocumentStatusId != 2) && d.DocumentStatusId == 1 && d.DocumentStepId == 8 && doctype.Contains(d.DocumentTypeId.Value)
                                    && ((d.QualityOn.HasValue))
                                    select new { d, f });



                //TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                // to be export
                var completedtotal = (from d in exorabilisContext.Document
                                      join f in exorabilisContext.Files on d.FileId equals f.Id
                                      join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                      where d.DocumentStatusId == 1 && d.QualityOn.HasValue && d.DocumentStepId == 8 && d.ExportStatus == 0 && doctype.Contains(d.DocumentTypeId.Value)
                                      select new { d, f });

                // exported
                var completedtotalexported = (from d in exorabilisContext.Document
                                              join f in exorabilisContext.Files on d.FileId equals f.Id
                                              join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                              where d.DocumentStatusId == 1 && d.ExportOn.HasValue && d.DocumentStepId == 8 && d.ExportStatus == 1 && doctype.Contains(d.DocumentTypeId.Value)
                                              select new { d, f });

                var rescantotal = (from d in exorabilisContext.Document
                                   join f in exorabilisContext.Files on d.FileId equals f.Id
                                   join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                   where (d.DocumentStatusId == 5) && doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue
                                   select new { d, f });

                var rescancompletedtotal = (from d in exorabilisContext.Document
                                            join f in exorabilisContext.Files on d.FileId equals f.Id
                                            join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                            where (d.DocumentStatusId == 6) && doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue
                                            select new { d, f });


                // for the users

                var scannedtotalbyuser = (from d in exorabilisContext.Document
                                          join f in exorabilisContext.Files on d.FileId equals f.Id
                                          join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                          where (d.DocumentStepId > 1) && doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue
                                          select new { d, f });


                var qualitytotalbyuser = (from d in exorabilisContext.Document
                                          join f in exorabilisContext.Files on d.FileId equals f.Id
                                          join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                          where doctype.Contains(d.DocumentTypeId.Value) && d.QualityOn.HasValue
                                          select new { d, f });


                var rescantotalbyuser = (from d in exorabilisContext.Document
                                         join f in exorabilisContext.Files on d.FileId equals f.Id
                                         join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                         where doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue
                                         select new { d, f });


                if (request.firstdate != null)
                {
                    // to be quality
                    scannedtotal = from d in scannedtotal
                                   where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
                                   select d;

                    // scan
                    scantotal = from d in scantotal
                                where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
                                select d;

                    // scan by user
                    scannedtotalbyuser = from d in scannedtotalbyuser
                                         where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
                                         select d;

                    // to be indexing
                    qualitytotal = from d in qualitytotal
                                   where
                                  (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) >= request.firstdate && d.d.LastStep == "Quality") ||
                                  (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate && d.d.LastStep == "Scan")
                                   select d;

                    // quality by user 
                    qualitytotalbyuser = from d in qualitytotalbyuser
                                         where d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) >= request.firstdate
                                         select d;


                    // to be rescan
                    rescantotal = from d in rescantotal
                                  where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
                                  select d;

                    // send to rescan by user
                    rescantotalbyuser = from d in rescantotalbyuser
                                        where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
                                        select d;

                    rescancompletedtotal = from d in rescancompletedtotal
                                           where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
                                           select d;

                    //TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                    // reviewed complete total ( to be export )
                    completedtotal = from d in completedtotal
                                     where d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) >= request.firstdate
                                     select d;

                    // exported TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                    completedtotalexported = from d in completedtotalexported
                                             where d.d.ExportOn.HasValue && DateOnly.FromDateTime(d.d.ExportOn.Value) >= request.firstdate
                                             select d;

                }

                if (request.lastdate != null)
                {
                    scantotal = from d in scantotal
                                where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate
                                select d;

                    scannedtotal = from d in scannedtotal
                                   where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate
                                   select d;

                    // scan by user
                    scannedtotalbyuser = from d in scannedtotalbyuser
                                         where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate
                                         select d;

                    // to be index
                    qualitytotal = from d in qualitytotal
                                   where
                                   (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate && d.d.LastStep == "Quality") ||
                                  (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate && d.d.LastStep == "Scan")
                                   select d;

                    // quality by user 
                    qualitytotalbyuser = from d in qualitytotalbyuser
                                         where d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate
                                         select d;



                    // to be rescan
                    rescantotal = from d in rescantotal
                                  where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                  select d;

                    // send to rescan by user
                    rescantotalbyuser = from d in rescantotalbyuser
                                        where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                        select d;

                    rescancompletedtotal = from d in rescancompletedtotal
                                           where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                           select d;

                    //TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                    // reviewed complete total
                    completedtotal = from d in completedtotal
                                     where d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate
                                     select d;

                    // exported  TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                    completedtotalexported = from d in completedtotalexported
                                             where d.d.ExportOn.HasValue && DateOnly.FromDateTime(d.d.ExportOn.Value) <= request.lastdate
                                             select d;


                }

                // scan complete

                var scannedtotalbyuser1 = scannedtotalbyuser.ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList();

                var scannedtotalfinalebyuser = (from d in scannedtotalbyuser1
                                                group d by d.d.CreatedBy into g
                                                select new
                                                {
                                                    Username = g.Key,
                                                    count = g.Count()
                                                }).ToList();


                // quality complete
                var qualitytotalbyuser1 = qualitytotalbyuser.ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
                   .GroupBy(x => x.f.Id) // Grouper par FileId
                   .Select(group =>
                   {
                       var isSingle = group.First().f.FileTypeId == 1;
                       return isSingle
                           ? group // Récupérer tous les documents si single
                           : group.Take(1); // Récupérer un seul document si multi
                   })
                   .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                   .ToList();

                var qualitytotalfinalebyuser = (from d in qualitytotalbyuser1
                                                group d by d.d.QualityBy into g
                                                select new
                                                {
                                                    Username = g.Key,
                                                    count = g.Count()
                                                }).ToList();

                // send to rescan

                var rescantotalbyuser1 = rescantotalbyuser.ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
                .GroupBy(x => x.f.Id) // Grouper par FileId
                .Select(group =>
                {
                    var isSingle = group.First().f.FileTypeId == 1;
                    return isSingle
                        ? group // Récupérer tous les documents si single
                        : group.Take(1); // Récupérer un seul document si multi
                })
                .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                .ToList();
                var rescantotalfinalebyuser = (from d in rescantotalbyuser1
                                               group d by d.d.RescanBy into g
                                               select new
                                               {
                                                   Username = g.Key,
                                                   count = g.Count()
                                               }).ToList();



                var allUsers = scannedtotalfinalebyuser.Select(x => x.Username)
                .Union(qualitytotalfinalebyuser.Select(x => x.Username))
                .Union(rescantotalfinalebyuser.Select(x => x.Username))
                .Distinct();

                // Fusionner les données par utilisateur
                var finalList = allUsers.Select(username => new UserCount
                {
                    Username = username,
                    Scan = scannedtotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0,
                    Quality = qualitytotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0,
                    Rescan = rescantotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0
                }).ToList();

                //sheets
                var scannedtotalfinalesheets = (from d in scannedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                //documents // to be quality
                var scannedtotalfinale = scannedtotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();

                //sheets
                var scantotalfinalesheets = (from d in scantotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                //documents // scan
                var scantotalfinale = scantotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();

                // to be indexed sheets
                var qualitytotalfinalesheets = (from d in qualitytotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                // to be indexed
                var qualitytotalfinale = qualitytotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();



                var rescantotalfinalesheets = (from d in rescantotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                var rescantotalfinale = rescantotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();

                // sheets
                var completedtotalfinalesheets = (from d in completedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                var completedtotalfinale = completedtotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();
                // sheets
                var completedtotalexportedfinalesheets = (from d in completedtotalexported select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                // documents
                var completedtotalexportedfinale = completedtotalexportedfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();
                //sheets
                var rescancompletedtotalfinalesheets = (from d in rescancompletedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder);
                var rescancompletedtotalfinale = rescancompletedtotalfinalesheets
                    .GroupBy(x => x.f.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().f.FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().Count();




                new Dashboard_DTO
                {

                    scannfini = $"{scantotalfinale} [ {scantotalfinalesheets.Count()} : sheets ] ",
                    qualitytotalfini = $"{qualitytotalfinale} [ {qualitytotalfinalesheets.Count()} : sheets ] ",
                    scannedtotalfini = $"{scannedtotalfinale} [ {scannedtotalfinalesheets.Count()} : sheets ] ",
                    rescantotalfini = $"{rescantotalfinale} [ {rescantotalfinalesheets.Count()} : sheets ] ",
                    rescancompletedtotalfini = $"{rescancompletedtotalfinale} [ {rescancompletedtotalfinalesheets.Count()} : sheets ] ",
                    completedtotalfini = $"{completedtotalfinale} [ {completedtotalfinalesheets.Count()} : sheets ] ",
                    completedtotalexportedfinale = $"{completedtotalexportedfinale} [ {completedtotalexportedfinalesheets.Count()} : sheets ] ",
                    UserCounts = finalList
                };


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



            //try
            //{
            //    var data = this.getAllBatchByStep(request);

            //    var doctypeid = GetValueParam("DefaultDocType").Value;
            //    var FolderToExport = GetValueParam("FolderToExport").Value;


            //    var docstatus = new List<long> { 1 };
            //    var docstep = new List<long> { 8 };
            //    var doctype = new List<long> { Convert.ToInt64(doctypeid) };

            //    var filteredDocuments = (from d in exorabilisContext.Document
            //                             join f in exorabilisContext.Files on d.FileId equals f.Id
            //                             join b in exorabilisContext.Batch on f.BatchId equals b.Id
            //                             join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
            //                             where docstatus.Contains(b.DocumentStatusId.Value)
            //                                   && docstep.Contains(b.DocumentStepId.Value)
            //                                   && docstep.Contains(d.DocumentStepId.Value)
            //                                   && docstatus.Contains(d.DocumentStatusId.Value)
            //                                   && doctype.Contains(d.DocumentTypeId.Value)
            //                             select new
            //                             {
            //                                 b,
            //                                 Document = d,
            //                                 f.FileTypeId,
            //                                 f.Id,
            //                                 IndexModels = exorabilisContext.DocumentIndex.Where(x => x.DocumentId == d.Id).ToList(),
            //                                 Images = exorabilisContext.Image
            //                                          .Where(img => img.DocumentId == d.Id)
            //                                          .ToList()
            //                             });


            //    if (request.firstdate != null)
            //    {
            //        filteredDocuments = from d in filteredDocuments
            //                            where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) >= request.firstdate)
            //                            select d;
            //    }

            //    if (request.lastdate != null)
            //    {
            //        filteredDocuments = from d in filteredDocuments
            //                            where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) <= request.lastdate)
            //                            select d;
            //    }

            //    if (request.batchnumber != null)
            //    {
            //        filteredDocuments = from d in filteredDocuments
            //                            where d.b.ReferenceNumber.Contains(request.batchnumber)
            //                            select d;
            //    }


            //    var resultforexcelandexportbyBatch = filteredDocuments.ToList().DistinctBy(x => x.Document.Id)
            //    .GroupBy(x => x.Id) // Grouper par FileId
            //    .Select(group =>
            //    {
            //        var isSingle = group.First().FileTypeId == 1;
            //        return isSingle
            //            ? group // Récupérer tous les documents si single
            //            : group.Take(1); // Récupérer un seul document si multi
            //    })
            //    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
            //    .ToList().OrderBy(i => i.Document.PageOrder);

            //    var allindex = this.exorabilisContext.Index.ToList();

            //    // Créer un fichier Excel
            //    using (var package = new ExcelPackage())
            //    {
            //        var worksheet = package.Workbook.Worksheets.Add("Documents");

            //        // Ajouter les en-têtes dynamiques basés sur allindex

            //        int col = 1;
            //        worksheet.Cells[1, col].Value = "N°";
            //        col++;
            //        if (EnteteAdditionnelle.isInExel == 1 && !string.IsNullOrEmpty(EnteteAdditionnelle.DocNumber))
            //        {
            //            worksheet.Cells[1, col].Value = EnteteAdditionnelle.DocNumber;
            //            col++;
            //        }


            //        foreach (var index in allindex)
            //        {
            //            worksheet.Cells[1, col].Value = index.Name;
            //            col++;

            //        }

            //        col = this.EnteteAdditionnelle(EnteteAdditionnelle, worksheet, col);


            //        // Appliquer un style pour les en-têtes
            //        using (var range = worksheet.Cells[1, 1, 1, col - 1])
            //        {
            //            range.Style.Font.Bold = true;
            //            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
            //        }

            //        filename = DateTime.Now.ToString("yyyyMMdd");

            //        // Remplir les données des documents
            //        int row = 2;
            //        int i = 1;
            //        foreach (var documentelement in resultforexcelandexportbyBatch)
            //        {
            //            if (!Directory.Exists(Folder))
            //            {
            //                Directory.CreateDirectory(Folder); // Crée le dossier si nécessaire
            //            }


            //            col = 1;
            //            worksheet.Cells[row, col].Value = i.ToString();
            //            col++;
            //            if (EnteteAdditionnelle.isInExel == 1 && !string.IsNullOrEmpty(EnteteAdditionnelle.DocNumber))
            //            {
            //                worksheet.Cells[row, col].Value = documentelement.Document.ReferenceNumber;
            //                col++;
            //            }
            //            foreach (var index in allindex)
            //            {

            //                var docindex = documentelement.IndexModels.Where(x => x.IndexId == index.Id).FirstOrDefault();
            //                if (docindex != null)
            //                {

            //                    worksheet.Cells[row, col].Value = docindex.Value;
            //                }
            //                else
            //                {
            //                    worksheet.Cells[row, col].Value = "";
            //                }


            //                col++;

            //            }

            //            col = EnteteAdditionnelleValue(EnteteAdditionnelle, worksheet, col, row, documentelement.Document);

            //            i++;
            //            row++;
            //        }
            //        string excelFilePath = Path.Combine(Folder, $"{filename}.xlsx");
            //        // Ajuster la largeur des colonnes
            //        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            //        FileInfo excelFile = new FileInfo(excelFilePath);
            //        package.SaveAs(excelFile);

            //        Console.WriteLine($"Fichier Excel créé avec succès à : {excelFilePath}");
            //    }


            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}



        }

        public async Task<Chrono> getChrono()
        {

            var chrono = new Chrono();
            try
            {

                chrono = await exorabilisContext.Chrono.FirstOrDefaultAsync();


                return chrono;
            }
            catch (Exception ex)
            {
                return chrono;
                throw new Exception();
            }
        }
        public bool Export(BatchSearchRequest request)
        {

            var doctypeid = GetValueParam("DefaultDocType").Value;
            var isYear = GetValueParam("YearCompletOnChangeName").Value;
            var imageextension = GetValueParam("imageExtension").Value;
            var EnteteAdditionnelle = GetValueParam("EnteteExcelAdditionnelle");
            var FolderToExport = GetValueParam("FolderToExport").Value;
            var ExportByBatchOrDocument = Convert.ToInt32(GetValueParam("PdfByBatch").Value);
            var ExcelForAllBatch = Convert.ToInt32(GetValueParam("OneExcelForAllBatch").Value);
            var isBase64 = getChrono().Result.DatabaseSaveImage;

            try
            {
                var data = from b in exorabilisContext.Batch
                           where b.DocumentStepId == 8
                                     && b.DocumentStatusId == 1
                                     && b.ExportStatus == 0
                                     && b.NumberOfDocument > 0
                           select b;

                if (request.firstdate != null)
                {
                    data = from d in data
                           where (d.QualityOn.HasValue && DateOnly.FromDateTime(d.QualityOn.Value) >= request.firstdate) || (d.ScannedOn.HasValue && DateOnly.FromDateTime(d.ScannedOn.Value) >= request.firstdate)
                           select d;

                }

                if (request.lastdate != null)
                {
                    data = from d in data
                           where (d.QualityOn.HasValue && DateOnly.FromDateTime(d.QualityOn.Value) <= request.lastdate) || (d.ScannedOn.HasValue && DateOnly.FromDateTime(d.ScannedOn.Value) <= request.lastdate)
                           select d;
                }

                if (request.batchnumber != null)
                {
                    data = from d in data
                           where d.ReferenceNumber.Contains(request.batchnumber)
                           select d;
                }

                string filename = "";

                var result = (from d in data select d).Distinct().ToList();

                var timestamp_export = DateTime.Now;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
                string times = DateTime.Now.ToString("HH-mm-ff");

                string Folder = Path.Combine(FolderToExport, timestamp, times);



                /*if (ExcelForAllBatch == 1)
                {

                    var docstatus = new List<long> { 1 };
                    var docstep = new List<long> { 8 };
                    var doctype = new List<long> { Convert.ToInt64(doctypeid) };

                    var filteredDocuments = (from d in exorabilisContext.Document
                                             join f in exorabilisContext.Files on d.FileId equals f.Id
                                             join b in exorabilisContext.Batch on f.BatchId equals b.Id
                                             join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                                             where b.ExportStatus == 0
                                                   && docstatus.Contains(b.DocumentStatusId.Value)
                                                   && docstep.Contains(b.DocumentStepId.Value)
                                                   && docstep.Contains(d.DocumentStepId.Value)
                                                   && docstatus.Contains(d.DocumentStatusId.Value)
                                                   && doctype.Contains(d.DocumentTypeId.Value)
                                             select new
                                             {
                                                 b,
                                                 Document = d,
                                                 f.FileTypeId,
                                                 f.Id,
                                                 IndexModels = exorabilisContext.DocumentIndex.Where(x => x.DocumentId == d.Id).ToList(),
                                                 Images = exorabilisContext.Image
                                                          .Where(img => img.DocumentId == d.Id)
                                                          .ToList()
                                             });


                    if (request.firstdate != null)
                    {
                        filteredDocuments = from d in filteredDocuments
                                            where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) >= request.firstdate)
                                            select d;
                    }

                    if (request.lastdate != null)
                    {
                        filteredDocuments = from d in filteredDocuments
                                            where (d.b.ReviewedOn.HasValue && DateOnly.FromDateTime(d.b.ReviewedOn.Value) <= request.lastdate)
                                            select d;
                    }

                    if (request.batchnumber != null)
                    {
                        filteredDocuments = from d in filteredDocuments
                                            where d.b.ReferenceNumber.Contains(request.batchnumber)
                                            select d;
                    }





                    var resultforexcelandexportbyBatch = filteredDocuments.ToList().DistinctBy(x => x.Document.Id)
                    .GroupBy(x => x.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().OrderBy(i => i.Document.PageOrder);

                    var allindex = this.exorabilisContext.Index.ToList();

                    // Créer un fichier Excel
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Documents");

                        // Ajouter les en-têtes dynamiques basés sur allindex

                        int col = 1;
                        worksheet.Cells[1, col].Value = "N°";
                        col++;
                        if (EnteteAdditionnelle.isInExel == 1 && !string.IsNullOrEmpty(EnteteAdditionnelle.DocNumber))
                        {
                            worksheet.Cells[1, col].Value = EnteteAdditionnelle.DocNumber;
                            col++;
                        }


                        foreach (var index in allindex)
                        {
                            worksheet.Cells[1, col].Value = index.Name;
                            col++;

                        }

                        col = this.EnteteAdditionnelle(EnteteAdditionnelle, worksheet, col);


                        // Appliquer un style pour les en-têtes
                        using (var range = worksheet.Cells[1, 1, 1, col - 1])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                        }

                        filename = DateTime.Now.ToString("yyyyMMdd");

                        // Remplir les données des documents
                        int row = 2;
                        int i = 1;
                        foreach (var documentelement in resultforexcelandexportbyBatch)
                        {
                            if (!Directory.Exists(Folder))
                            {
                                Directory.CreateDirectory(Folder); // Crée le dossier si nécessaire
                            }


                            col = 1;
                            worksheet.Cells[row, col].Value = i.ToString();
                            col++;
                            if (EnteteAdditionnelle.isInExel == 1 && !string.IsNullOrEmpty(EnteteAdditionnelle.DocNumber))
                            {
                                worksheet.Cells[row, col].Value = documentelement.Document.ReferenceNumber;
                                col++;
                            }
                            foreach (var index in allindex)
                            {

                                var docindex = documentelement.IndexModels.Where(x => x.IndexId == index.Id).FirstOrDefault();
                                if (docindex != null)
                                {
                                    //if (index.IsFileName == 1)
                                    //{
                                    //    if (filename == "")
                                    //    {
                                    //        filename += docindex.Value;
                                    //    }
                                    //    else
                                    //    {
                                    //        filename += "-" + docindex.Value;
                                    //    }
                                    //}

                                    worksheet.Cells[row, col].Value = docindex.Value;
                                }
                                else
                                {
                                    worksheet.Cells[row, col].Value = "";
                                }


                                col++;

                            }

                            col = EnteteAdditionnelleValue(EnteteAdditionnelle, worksheet, col, row, documentelement.Document);

                            i++;
                            row++;
                        }
                        string excelFilePath = Path.Combine(Folder, $"{filename}.xlsx");
                        // Ajuster la largeur des colonnes
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        FileInfo excelFile = new FileInfo(excelFilePath);
                        package.SaveAs(excelFile);

                        Console.WriteLine($"Fichier Excel créé avec succès à : {excelFilePath}");
                    }

                }*/


                var j = 1;

                foreach (var item in result)
                {


                    filename = "";

                    var docstatus = new List<long> { 1 };
                    var docstep = new List<long> { 8 };
                    var doctype = new List<long> { Convert.ToInt64(doctypeid) };

                    var filteredDocuments = (from d in exorabilisContext.Document
                                             join f in exorabilisContext.Files on d.FileId equals f.Id
                                             join docFolio in exorabilisContext.Project_folio on f.Project_folio_id equals docFolio.Id into folioGroup
                                             from docFolio in folioGroup.DefaultIfEmpty()
                                             join docType in exorabilisContext.Document_type on docFolio.Document_type_id equals docType.Id into typeGroup
                                             from docType in typeGroup.DefaultIfEmpty()
                                             join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                                             where d.BatchId == item.Id
                                                   && docstep.Contains(d.DocumentStepId.Value)
                                                   && docstatus.Contains(d.DocumentStatusId.Value)
                                                   && doctype.Contains(d.DocumentTypeId.Value)
                                             select new
                                             {
                                                 doctype = docType,
                                                 docfolio = docFolio,
                                                 Document = d,
                                                 f.FileTypeId,
                                                 f.Id,
                                                 f.IsFramework,
                                                 IndexModels = exorabilisContext.DocumentIndex.Where(x => x.DocumentId == d.Id).ToList(),
                                                 Images = exorabilisContext.Image
                                                          .Where(img => img.DocumentId == d.Id)
                                                          .ToList()
                                             }).ToList().DistinctBy(x => x.Document.Id).OrderBy(x => x.Document.PageOrder);


                    var resultforexcelandexportbyBatch = filteredDocuments
                    .GroupBy(x => x.Id) // Grouper par FileId
                    .Select(group =>
                    {
                        var isSingle = group.First().FileTypeId == 1;
                        return isSingle
                            ? group // Récupérer tous les documents si single
                            : group.Take(1); // Récupérer un seul document si multi
                    })
                    .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                    .ToList().OrderBy(i => i.Document.PageOrder);

                    var allindex = this.exorabilisContext.Index.ToList();

                    if (ExportByBatchOrDocument != 0)
                    {
                        //var refbatch = (ExportByBatchOrDocument == 1) ? ReplaceInvalidChars(item.ReferenceNumber , isYear.ToString()) : ReplaceInvalidChars(item.FileNumber, isYear.ToString());

                        //string outputPath = Path.Combine(Folder, $"{refbatch}.pdf");

                        //var documentLayout = new PdfDocument();
                        ////var tesseractEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

                        //foreach (var item1 in filteredDocuments)
                        //{
                        //    var document = item1.Document;
                        //    var images = item1.Images;

                        //    document.ExportStatus = 1;
                        //    document.ExportOn = timestamp_export;

                        //    // Gestion des images RECTO et VERSO
                        //    var imageRecto = images.FirstOrDefault(x => x.Side == "RECTO" && x.Status == 1);
                        //    var imageVerso = images.FirstOrDefault(x => x.Side == "VERSO" && x.Status == 1);

                        //    string pathRecto = imageRecto?.Path ?? string.Empty;
                        //    string pathVerso = imageVerso?.Path ?? string.Empty;

                        //    string imageContentStr = imageRecto == null ? string.Empty : $"data:image/{imageextension};base64,{imageRecto.ImageToBase64String}";
                        //    string imageVersoContentStr = imageVerso == null ? string.Empty : $"data:image/{imageextension};base64,{imageVerso.ImageToBase64String}";

                        //    string recto = pathRecto;
                        //    string verso = pathVerso;

                        //    if (isBase64 == 1)
                        //    {
                        //        recto = imageContentStr;
                        //        verso = imageVersoContentStr;

                        //        byte[] imageBytesrecto = Convert.FromBase64String(recto);
                        //        byte[] imageBytesverso = Convert.FromBase64String(verso);

                        //        //pdf.AddNewPage();

                        //        if (imageBytesrecto.Length > 0)
                        //        {
                        //            AddImage(documentLayout, imageBytesrecto);

                        //        }

                        //        // Ajouter une image VERSO si elle existe
                        //        if (imageBytesverso.Length > 0)
                        //        {
                        //            //pdf.AddNewPage(); // Une nouvelle page pour le verso
                        //            AddImage(documentLayout, imageBytesverso);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        string webRootPath = request.webhostEnvironment.WebRootPath;

                        //        //pdf.AddNewPage();

                        //        if (!string.IsNullOrEmpty(recto))
                        //        {
                        //            AddImagePath(documentLayout, recto, webRootPath);
                        //        }

                        //        // Ajouter une image VERSO si elle existe
                        //        if (!string.IsNullOrEmpty(verso))
                        //        {
                        //            //pdf.AddNewPage(); // Une nouvelle page pour le verso
                        //            AddImagePath(documentLayout, verso, webRootPath);

                        //        }
                        //    }
                        //    //string output = Path.Combine(Folder, "pdfscanned.pdf");
                        //    //setpdfsearchableAsync(output, outputPath);

                        //}

                        //item.ExportStatus = 1;
                        //item.ExportOn = timestamp_export;

                        //documentLayout.Save(outputPath);

                    }
                    else
                    {

                        foreach (var item1 in resultforexcelandexportbyBatch)
                        {
                            // si c'est framework
                            if (item1.IsFramework == 1)
                            {

                            }
                            // si c'est pas framework
                            else
                            {
                                filename = item1.doctype.pdf_name;


                                string Documentfolder = Path.Combine(Folder, ReplaceChars(item.FileNumber), ReplaceChars(item1.doctype.folder_name));

                                string outputPath = Path.Combine(Documentfolder, $"{filename}.pdf");



                                if (item1.docfolio.Folios.Contains("PF"))
                                {
                                    string firstDocFolder = Documentfolder;
                                    Documentfolder = Documentfolder + " PF";
                                    outputPath = Path.Combine(Documentfolder, $"{filename} PF.pdf");
                                    int compteur = 1;

                                    while (Directory.Exists(Documentfolder))
                                    {
                                        Documentfolder = firstDocFolder + $" PF{compteur}";
                                        outputPath = Path.Combine(Documentfolder, $"{filename} PF{compteur}.pdf");
                                        compteur++;
                                    }

                                    Directory.CreateDirectory(Documentfolder);


                                }
                                else
                                {
                                    if (!Directory.Exists(Documentfolder))
                                    {
                                        Directory.CreateDirectory(Documentfolder); // Crée le dossier si nécessaire
                                    }

                                    outputPath = Path.Combine(Documentfolder, $"{filename}.pdf");

                                    int compteur = 1;

                                    while (File.Exists(outputPath))
                                    {
                                        outputPath = Path.Combine(Documentfolder, $"{filename}_{compteur}.pdf");
                                        compteur++;
                                    }
                                }



                                if (item1.FileTypeId == 2)
                                {
                                    var filteredDocumentsbyFile = (from d in exorabilisContext.Document
                                                                   join f in exorabilisContext.Files on d.FileId equals f.Id
                                                                   join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                                                                   where d.FileId == item1.Id && ir.Status == 1
                                                                         && docstep.Contains(d.DocumentStepId.Value)
                                                                         && docstatus.Contains(d.DocumentStatusId.Value)
                                                                         && doctype.Contains(d.DocumentTypeId.Value)
                                                                   select new
                                                                   {
                                                                       Document = d,
                                                                       f.FileTypeId,
                                                                       f.Id,
                                                                       IndexModels = exorabilisContext.DocumentIndex.Where(x => x.DocumentId == d.Id).ToList(),
                                                                       Images = exorabilisContext.Image
                                                                                .Where(img => img.DocumentId == d.Id)
                                                                                .ToList()
                                                                   }).ToList().DistinctBy(C => C.Document.Id).OrderBy(i => i.Document.PageOrder);

                                    var documentLayout = new PdfDocument();

                                    foreach (var item2 in filteredDocumentsbyFile)
                                    {
                                        var document = item2.Document;
                                        var images = item2.Images;

                                        document.ExportStatus = 1;
                                        document.ExportOn = timestamp_export;

                                        // Gestion des images RECTO et VERSO
                                        var imageRecto = images.FirstOrDefault(x => x.Side == "RECTO");
                                        var imageVerso = images.FirstOrDefault(x => x.Side == "VERSO");

                                        string pathRecto = imageRecto?.Path ?? string.Empty;
                                        string pathVerso = imageVerso?.Path ?? string.Empty;

                                        string imageContentStr = imageRecto == null ? string.Empty : $"data:image/{imageextension};base64,{imageRecto.ImageToBase64String}";
                                        string imageVersoContentStr = imageVerso == null ? string.Empty : $"data:image/{imageextension};base64,{imageVerso.ImageToBase64String}";

                                        string recto = pathRecto;
                                        string verso = pathVerso;

                                        if (isBase64 == 1)
                                        {
                                            recto = imageContentStr;
                                            verso = imageVersoContentStr;

                                            byte[] imageBytesrecto = Convert.FromBase64String(recto);
                                            byte[] imageBytesverso = Convert.FromBase64String(verso);

                                            if (imageBytesrecto.Length > 0)
                                            {
                                                AddImage(documentLayout, imageBytesrecto);

                                            }

                                            if (imageBytesverso.Length > 0)
                                            {
                                                AddImage(documentLayout, imageBytesverso);
                                            }
                                        }
                                        else
                                        {
                                            string webRootPath = request.webhostEnvironment.WebRootPath;


                                            if (!string.IsNullOrEmpty(recto))
                                            {
                                                AddImagePath(documentLayout, recto, webRootPath);
                                            }

                                            if (!string.IsNullOrEmpty(verso))
                                            {
                                                AddImagePath(documentLayout, verso, webRootPath);

                                            }
                                        }

                                    }

                                    documentLayout.Save(outputPath);


                                }
                            }


                            item.ExportStatus = 1;
                        }
                    }

                    j++;

                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.StackTrace);
            }


        }

        private string ReplaceChars(string fileName)
        {

            char[] InvalidChars = new char[] { '/', '\\', '*', '"', '<', '>', '|', ':', '?', '\0' };

            // On divise la chaîne en morceaux valides
            string[] parts = fileName.Split(InvalidChars, StringSplitOptions.RemoveEmptyEntries);

            // On les rejoint avec un tiret
            string cleanFileName = string.Join("-", parts);

            return cleanFileName;
        }

        private string ReplaceInvalidChars(string fileName, string isYear)
        {

            char[] InvalidChars = new char[] { '/', '\\', '*', '"', '<', '>', '|', ':', '?', '\0' };

            foreach (char invalidChar in InvalidChars)
            {
                fileName = fileName.Replace(invalidChar.ToString(), "-");
            }

            // Retire "MU-M-" si déjà présent
            string cleanName = fileName.Replace("MU-M-", "");

            string[] parts = cleanName.Split('-', 2);
            if (parts.Length == 2)
            {
                string rawYear = parts[0];
                string rest = parts[1];

                if (int.TryParse(rawYear, out int yearNum))
                {
                    string newYear;
                    if (isYear == "1")
                    {
                        newYear = yearNum < 100 ? $"20{yearNum:D2}" : yearNum.ToString();
                    }
                    else
                    {
                        newYear = (yearNum % 100).ToString("D2");
                    }

                    fileName = $"MU-M-{newYear}-{rest}";

                }
            }
            else
            {
                throw new ArgumentException("Invalid file name format");
            }

            return fileName;
        }

        static int EnteteAdditionnelleValue(Parameter EnteteAdditionnelle, ExcelWorksheet worksheet, int col, int row, Core.Entity.Exorabilis.Document documentelement)
        {
            if (EnteteAdditionnelle.isInExel == 1)
            {
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.QualityName))
                {
                    worksheet.Cells[row, col].Value = documentelement.QualityBy;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.QualityOn))
                {
                    worksheet.Cells[row, col].Value = documentelement.QualityOn.HasValue ? documentelement.QualityOn.Value : "";
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.IndexedName))
                {
                    worksheet.Cells[row, col].Value = documentelement.IndexedBy;

                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.IndexedOn))
                {
                    worksheet.Cells[row, col].Value = documentelement.IndexedOn.HasValue ? documentelement.IndexedOn.Value : "";
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";


                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.SanityName))
                {
                    worksheet.Cells[row, col].Value = documentelement.SanityBy;

                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.SanityOn))
                {
                    worksheet.Cells[row, col].Value = documentelement.SanityOn.HasValue ? documentelement.SanityOn.Value : "";
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";

                    col++;
                }

                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ModifiedName))
                {
                    worksheet.Cells[row, col].Value = documentelement.ModifiedBy;

                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ModifiedOn))
                {
                    worksheet.Cells[row, col].Value = documentelement.ModifiedOn.HasValue ? documentelement.ModifiedOn.Value : "";
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";

                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ReviewedName))
                {
                    worksheet.Cells[row, col].Value = documentelement.ReviewedBy;

                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ReviewedOn))
                {
                    worksheet.Cells[row, col].Value = documentelement.ReviewedOn.HasValue ? documentelement.ReviewedOn.Value : "";
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";

                    col++;
                }
            }

            return col;
        }

        public int EnteteAdditionnelle(Parameter EnteteAdditionnelle, ExcelWorksheet worksheet, int col)
        {
            if (EnteteAdditionnelle.isInExel == 1)
            {
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.QualityName))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.QualityName;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.QualityOn))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.QualityOn;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.IndexedName))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.IndexedName;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.IndexedOn))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.IndexedOn;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.SanityName))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.SanityName;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.SanityOn))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.SanityOn;
                    col++;
                }

                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ModifiedName))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.ModifiedName;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ModifiedOn))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.ModifiedOn;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ReviewedName))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.ReviewedName;
                    col++;
                }
                if (!string.IsNullOrEmpty(EnteteAdditionnelle.ReviewedOn))
                {
                    worksheet.Cells[1, col].Value = EnteteAdditionnelle.ReviewedOn;
                    col++;
                }
            }

            return col;
        }



        private static void AddImagePath(PdfDocument documentLayout, string recto, string webRootPath)
        {

            string fullPath = webRootPath + recto;


            FileStream imageStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var image = XImage.FromStream(imageStream);

            // 🔹 Création d'une nouvelle page avec la taille de l'image
            var page = documentLayout.AddPage();
            page.Width = image.PixelWidth * 72 / image.HorizontalResolution; // Conversion des pixels en points
            page.Height = image.PixelHeight * 72 / image.VerticalResolution;

            var gfx = XGraphics.FromPdfPage(page);
            gfx.DrawImage(image, 0, 0, page.Width, page.Height);
        }

        private static void AddImage(PdfDocument documentLayout, byte[] imageBytesverso)
        {
            using (var ms = new MemoryStream(imageBytesverso))
            {
                var page = documentLayout.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var image = XImage.FromStream(ms);

                gfx.DrawImage(image, 0, 0, page.Width, page.Height);
            }

        }

        public async Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchById(string? batchId)
        {
            try
            {

                var IdBatch = Convert.ToInt64(batchId);

                var data = (from b in exorabilisContext.Batch
                            join status in exorabilisContext.DocumentStatus on b.DocumentStatusId equals status.Id
                            join step in exorabilisContext.DocumentStep on b.DocumentStepId equals step.Id
                            join type in exorabilisContext.DocumentType on b.DocumentStatusId equals type.Id
                            join file in exorabilisContext.Files on b.Id equals file.BatchId // assuming you want to join with File for the FileType
                            where b.Id == IdBatch
                            select new Batch_Quality_Crop_Index_Sanity_Unlock_DTO
                            {
                                BatchNumber = b.ReferenceNumber,
                                NumberOfDocument = b.NumberOfDocument,
                                StatusName = status.Name,
                                FileNumber = b.FileNumber ?? "",
                                StepName = step.Name,
                                TypeName = type.Name,
                                ScannedOn = b.ScannedOn,
                                IsLocked = b.IsLocked,
                                IdEncrypt = b.Id.ToString(),
                                LockedBy = b.LockedBy ?? "",
                                FileTypeName = file.FileType.Name ?? "",
                                FileTypeId = file.FileTypeId
                            }
                           ).FirstOrDefault();



                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        async Task setpdfsearchableAsync(string pathpdf, string pathtoexport)
        {
            //try
            //{
            //    string tesseractData = Path.GetFullPath(@"C:\Program Files\Tesseract-OCR\tessdata\");
            //    // Paths
            //    string inputPdfPath = pathpdf; // Path to the non-searchable PDF
            //    string outputPdfPath = pathtoexport; // Path to save the searchable PDF
            //    string tessDataPath = tesseractData; // Path to the Tesseract language data folder

            //    var convertApi = new ConvertApi("secret_KuoWQep7ScTkow6k");
            //    var convert = await convertApi.ConvertAsync("pdf", "merge",
            //        new ConvertApiFileParam("Files", pathpdf)

            //    );
            //    await convert.SaveFilesAsync(@"C:\converted-files\");
            //}
            //catch (Exception)
            //{

            //    throw;
            //}


            //string tesseractLanguages = "fra";
            //string tesseractData = Path.GetFullPath(@"C:\Program Files\Tesseract-OCR\tessdata\");
            //string tempFile = Path.Combine(tesseractData, Path.GetRandomFileName());
            ////SautinSoft.Pdf.PdfDocument pdfDocument = SautinSoft.Pdf.PdfDocument.Load($"{pathpdf}");

            //using (SautinSoft.Pdf.PdfDocument pdfDocument = SautinSoft.Pdf.PdfDocument.Load(pathpdf))
            //{
            //    try
            //    {

            //        // Now you can work with pdfDocument here
            //        Console.WriteLine("PDF loaded successfully!");
            //        // You can perform operations on pdfDocument, e.g., extracting text or processing pages.
            //        PdfFormattedText text = new PdfFormattedText();

            //        using (Tesseract.TesseractEngine engine = new Tesseract.TesseractEngine(tesseractData, tesseractLanguages, Tesseract.EngineMode.Default))
            //        {
            //            foreach (var pdfPage in pdfDocument.Pages)
            //            {
            //                var collection = pdfPage.Content.Elements.All().OfType<PdfImageContent>().ToList();
            //                for (int i = 0; i < collection.Count(); i++)
            //                {
            //                    engine.DefaultPageSegMode = Tesseract.PageSegMode.Auto;
            //                    using (MemoryStream ms = new MemoryStream())
            //                    {
            //                        collection[i].Save(ms, new ImageSaveOptions());

            //                        byte[] imgBytes = ms.ToArray();
            //                        using (Tesseract.Pix img = Tesseract.Pix.LoadFromMemory(imgBytes))
            //                        {
            //                            using (var page = engine.Process(img, "Serachablepdf"))
            //                            {
            //                                var st = page.GetText();
            //                                double scale = Math.Min(collection[i].Bounds.Width / page.RegionOfInterest.Width, collection[i].Bounds.Height / page.RegionOfInterest.Height);

            //                                using (var iter = page.GetIterator())
            //                                {
            //                                    iter.Begin();

            //                                    do
            //                                    {
            //                                        do
            //                                        {
            //                                            do
            //                                            {
            //                                                do
            //                                                {
            //                                                    iter.TryGetBoundingBox(PageIteratorLevel.Word, out Rect liRect);
            //                                                    text.FontSize = liRect.Height * scale;
            //                                                    text.Opacity = 0;
            //                                                    text.Append(iter.GetText(PageIteratorLevel.Word));
            //                                                    pdfPage.Content.DrawText(text, new PdfPoint(collection[i].Bounds.Left + liRect.X1 * scale, collection[i].Bounds.Top - liRect.Y1 * scale - text.Height));
            //                                                    text.Clear();
            //                                                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
            //                                            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
            //                                        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
            //                                    } while (iter.Next(PageIteratorLevel.Block));
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        pdfDocument.Save(pathtoexport);
            //        //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("Result.pdf") { UseShellExecute = true });
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine();
            //        Console.WriteLine("Please be sure that you have Language data files (*.traineddata) in your folder \"tessdata\"");
            //        Console.WriteLine("The Language data files can be download from here: https://github.com/tesseract-ocr/tessdata_fast");
            //        Console.ReadKey();
            //        //throw new Exception("Error Tesseract: " + e.Message);
            //    }
            //    finally
            //    {

            //    }
            //}


        }

        public async Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchByRef(string batchReference)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, string)> IsBatchLocked(BatchwithStepRequest batches, string username)
        {
            try
            {
                var response = (false, "");

                var data = Convert.ToInt64(batches.batchId);

                Batch batch = await exorabilisContext.Batch.Where(x => x.Id == data).FirstOrDefaultAsync();
                if (batch != null)
                {

                    if ((batch.DocumentStatusId != 5 && batch.DocumentStatusId != 6 && batch.DocumentStatusId != 1 && batches.step != 11 && batches.step != 12 && batches.step != 8))
                    {
                        var isLocked = (batch.IsLocked.HasValue && batch.IsLocked.Value == 1 && batch.LockedBy != username) ? true : false;

                        if (!isLocked)
                        {
                            GlobalVariables.StartDate = DateTime.Now;
                        }

                        response = (isLocked, batch.LockedBy);
                    }

                }

                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Parameter GetValueParam(string name)
        {
            Parameter parameter = new Parameter();

            try
            {
                var param = (from s in exorabilisContext.Parameter
                             where s.Name == name
                             select s).FirstOrDefault();
                if (param != null)
                {
                    parameter = param;
                }

                return parameter;
            }
            catch (Exception ex)
            {
                return parameter; // Ensure a value is returned in case of an exception
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> MakeCompletedBatch(BatchwithStepRequest request)
        {
            var transaction = this.exorabilisContext.Database.BeginTransaction();
            GlobalVariables.StartDate = DateTime.Now;
            try
            {
                var response = true;


                switch (request.type)
                {
                    case 1:  // complete one document 
                        var dataDOC = Convert.ToInt64(request.docId);

                        var documents = (from d in exorabilisContext.Document
                                         join f in exorabilisContext.Files on d.FileId equals f.Id
                                         where d.Id == dataDOC
                                         select new { d, f }).FirstOrDefault();

                        if (documents.d != null)
                        {
                            if (documents.f.FileTypeId == 2)
                            {
                                var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == documents.f.Id).ToList();

                                if (multiDocuments != null)
                                {
                                    foreach (var multiDocument in multiDocuments)
                                    {
                                        multiDocument.DocumentStatusId = 6;
                                        multiDocument.RescanBy = request.UserName;
                                        multiDocument.RescanOn = DateTime.Now;

                                        multiDocument.LastStep = "Rescan";

                                        var multiDocumentHistory = new DocumentHistory();
                                        multiDocumentHistory.BatchId = null;
                                        multiDocumentHistory.DocumentId = multiDocument.Id;
                                        multiDocumentHistory.DocumentStepId = multiDocument.DocumentStepId;
                                        multiDocumentHistory.DocumentStatusId = multiDocument.DocumentStatusId;
                                        multiDocumentHistory.RejectionCodeId = multiDocument.RejectionCodeId;
                                        multiDocumentHistory.UserId = request.UserId.ToString();
                                        multiDocumentHistory.UserName = request.UserName;

                                        exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                                    }
                                }
                            }
                            else
                            {
                                var documenta = exorabilisContext.Document.Where(x => x.Id == dataDOC).FirstOrDefault();

                                documenta.DocumentStatusId = 6;
                                documenta.RescanBy = request.UserName;
                                documenta.RescanOn = DateTime.Now;
                                documenta.LastStep = "Rescan";

                                var multiDocumentHistory = new DocumentHistory();
                                multiDocumentHistory.BatchId = null;
                                multiDocumentHistory.DocumentId = documenta.Id;
                                multiDocumentHistory.DocumentStepId = documenta.DocumentStepId;
                                multiDocumentHistory.DocumentStatusId = documenta.DocumentStatusId;
                                multiDocumentHistory.RejectionCodeId = documenta.RejectionCodeId;
                                multiDocumentHistory.UserId = request.UserId.ToString();
                                multiDocumentHistory.UserName = request.UserName;

                                exorabilisContext.DocumentHistory.Add(multiDocumentHistory);

                            }

                        }
                        break;
                    case 2: // complete all document not completed


                        var doctypeid = GetValueParam("DefaultDocType").Value;


                        List<long> docstatus = new List<long> { 5 };
                        List<long> docstep = new List<long> { 8 };
                        List<long> doctype = new List<long> { Convert.ToInt64(doctypeid) };


                        var datas = (from d in exorabilisContext.Document
                                     join f in exorabilisContext.Files on d.FileId equals f.Id
                                     join ftype in exorabilisContext.FileType on f.FileTypeId equals ftype.Id
                                     join b in exorabilisContext.Batch on f.BatchId equals b.Id
                                     join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                                     join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id
                                     join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id
                                     join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id
                                     join code in exorabilisContext.RejectionCode on d.RejectionCodeId equals code.Id into reject
                                     from rejcode in reject.DefaultIfEmpty()
                                     where (b.DocumentStatusId != 5 && b.DocumentStatusId != 6) && docstep.Contains(d.DocumentStepId.Value) && docstatus.Contains(d.DocumentStatusId.Value) && doctype.Contains(d.DocumentTypeId.Value)

                                     select new
                                     {
                                         d,
                                         b,
                                         f

                                     });

                        if (request.firstdate != null)
                        {
                            datas = from d in datas
                                    where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
                                    select d;
                        }

                        if (request.lastdate != null)
                        {
                            datas = from d in datas
                                    where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                    select d;
                        }

                        if (request.batchnumber != null)
                        {
                            datas = from d in datas
                                    where d.b.ReferenceNumber.Contains(request.batchnumber)
                                    select d;
                        }

                        if (request.documentnumber != null)
                        {
                            datas = from d in datas
                                    where d.d.ReferenceNumber.Contains(request.documentnumber)
                                    select d;
                        }

                        var resultat = (from d in datas
                                        select new
                                        {
                                            FileId = d.f.Id,
                                            Id = d.d.Id,
                                            FileTypeId = d.b.File.FileTypeId,

                                        }).ToList().DistinctBy(x => x.Id);




                        var results = resultat
                        .GroupBy(x => x.FileId) // Grouper par FileId
                        .Select(group =>
                        {
                            var isSingle = group.First().FileTypeId == 1;
                            return isSingle
                                ? group // Récupérer tous les documents si single
                                : group.Take(1); // Récupérer un seul document si multi
                        })
                        .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                        .ToList();


                        foreach (var item in results)
                        {
                            var docid = item.Id;

                            var document = exorabilisContext.Document.Where(x => x.Id == docid).FirstOrDefault();

                            if (document != null)
                            {

                                if (item.FileTypeId == 2)
                                {
                                    var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                                    if (multiDocuments != null)
                                    {
                                        foreach (var multiDocument in multiDocuments)
                                        {
                                            multiDocument.DocumentStatusId = 6;
                                            multiDocument.RescanBy = request.UserName;
                                            multiDocument.RescanOn = DateTime.Now;

                                            multiDocument.LastStep = "Rescan";

                                            var multiDocumentHistory = new DocumentHistory();
                                            multiDocumentHistory.BatchId = null;
                                            multiDocumentHistory.DocumentId = multiDocument.Id;
                                            multiDocumentHistory.DocumentStepId = multiDocument.DocumentStepId;
                                            multiDocumentHistory.DocumentStatusId = multiDocument.DocumentStatusId;
                                            multiDocumentHistory.RejectionCodeId = multiDocument.RejectionCodeId;
                                            multiDocumentHistory.UserId = request.UserId.ToString();
                                            multiDocumentHistory.UserName = request.UserName;

                                            exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                                        }
                                    }
                                }
                                else
                                {
                                    document.DocumentStatusId = 6;
                                    document.RescanBy = request.UserName;
                                    document.RescanOn = DateTime.Now;
                                    document.LastStep = "Rescan";

                                    var multiDocumentHistory = new DocumentHistory();
                                    multiDocumentHistory.BatchId = null;
                                    multiDocumentHistory.DocumentId = document.Id;
                                    multiDocumentHistory.DocumentStepId = document.DocumentStepId;
                                    multiDocumentHistory.DocumentStatusId = document.DocumentStatusId;
                                    multiDocumentHistory.RejectionCodeId = document.RejectionCodeId;
                                    multiDocumentHistory.UserId = request.UserId.ToString();
                                    multiDocumentHistory.UserName = request.UserName;

                                    exorabilisContext.DocumentHistory.Add(multiDocumentHistory);

                                }

                            }



                        }



                        break;
                    case 3: // complete all batch



                        var data = from b in exorabilisContext.Batch
                                   join status in exorabilisContext.DocumentStatus on b.DocumentStatusId equals status.Id
                                   join step in exorabilisContext.DocumentStep on b.DocumentStepId equals step.Id
                                   join type in exorabilisContext.DocumentType on b.DocumentStatusId equals type.Id
                                   join reject in exorabilisContext.RejectionCode on b.RejectionCodeId equals reject.Id into rejected
                                   from reject in rejected.DefaultIfEmpty()
                                   join file in exorabilisContext.Files on b.Id equals file.BatchId // assuming you want to join with File for the FileType
                                   select b;

                        if (request.firstdate != null)
                        {
                            data = from d in data
                                   where d.RescanOn.HasValue && DateOnly.FromDateTime(d.RescanOn.Value) >= request.firstdate
                                   select d;
                        }

                        if (request.lastdate != null)
                        {
                            data = from d in data
                                   where d.RescanOn.HasValue && DateOnly.FromDateTime(d.RescanOn.Value) <= request.lastdate
                                   select d;
                        }

                        if (request.batchnumber != null)
                        {
                            data = from d in data
                                   where d.ReferenceNumber.Contains(request.batchnumber)
                                   select d;
                        }



                        var result = await (from d in data
                                            where d.DocumentStepId != 1
                                       && (d.DocumentStatusId == 5)
                                         && d.RejectionCodeId.HasValue
                                         && d.NumberOfDocument > 0
                                            select d).Distinct().ToListAsync();


                        foreach (var item in result)
                        {
                            item.DocumentStatusId = 6;

                            (from d in exorabilisContext.Document
                             join f in exorabilisContext.Files on d.FileId equals f.Id
                             where f.BatchId == item.Id
                             select d).ToList().ForEach(x =>
                             {
                                 x.DocumentStatusId = 6;

                                 var multiDocumentHistory = new DocumentHistory();
                                 multiDocumentHistory.BatchId = item.Id;
                                 multiDocumentHistory.DocumentId = x.Id;
                                 multiDocumentHistory.DocumentStepId = x.DocumentStepId;
                                 multiDocumentHistory.DocumentStatusId = 6;
                                 multiDocumentHistory.RejectionCodeId = x.RejectionCodeId;
                                 multiDocumentHistory.UserId = request.UserId.ToString();
                                 multiDocumentHistory.UserName = request.UserName;
                                 multiDocumentHistory.RejectionCodeId = x.RejectionCodeId;

                                 exorabilisContext.DocumentHistory.Add(multiDocumentHistory);


                             });

                            BatchHistory history = new BatchHistory();
                            history.BatchId = item.Id;
                            history.DocumentStatusId = item?.DocumentStatusId;
                            history.DocumentStepId = item?.DocumentStepId;
                            history.RejectionCode = item?.RejectionCodeId;
                            history.NumberOfDocument = item?.NumberOfDocument;
                            history.UserId = request.UserId;
                            history.Remark = item?.ReasonOther;
                            history.StartedOn = GlobalVariables.StartDate;
                            history.EndedOn = DateTime.Now;
                            this.exorabilisContext.AddAsync(history);

                        }
                        break;
                    case 4: // complete one batch
                        var dataone = Convert.ToInt64(request.batchId);

                        Batch batch = exorabilisContext.Batch.Where(x => x.Id == dataone).FirstOrDefault();

                        if (batch != null)
                        {
                            batch.DocumentStatusId = 6;

                            (from d in exorabilisContext.Document
                             join f in exorabilisContext.Files on d.FileId equals f.Id
                             where f.BatchId == batch.Id
                             select d).ToList().ForEach(x =>
                             {
                                 x.DocumentStatusId = 6;

                                 var multiDocumentHistory = new DocumentHistory();
                                 multiDocumentHistory.BatchId = batch.Id;
                                 multiDocumentHistory.DocumentId = x.Id;
                                 multiDocumentHistory.DocumentStepId = x.DocumentStepId;
                                 multiDocumentHistory.DocumentStatusId = 6;
                                 multiDocumentHistory.RejectionCodeId = x.RejectionCodeId;
                                 multiDocumentHistory.UserId = request.UserId.ToString();
                                 multiDocumentHistory.UserName = request.UserName;
                                 multiDocumentHistory.RejectionCodeId = x.RejectionCodeId;

                                 exorabilisContext.DocumentHistory.Add(multiDocumentHistory);


                             });

                            BatchHistory history = new BatchHistory();
                            history.BatchId = batch.Id;
                            history.DocumentStatusId = batch?.DocumentStatusId;
                            history.DocumentStepId = batch?.DocumentStepId;
                            history.RejectionCode = batch?.RejectionCodeId;
                            history.NumberOfDocument = batch?.NumberOfDocument;
                            history.UserId = request.UserId;
                            history.StartedOn = GlobalVariables.StartDate;
                            history.Remark = batch.ReasonOther;
                            history.EndedOn = DateTime.Now;
                            this.exorabilisContext.AddAsync(history);
                        }
                        break;
                    case 6:  // unlock one batch
                        var batchId = Convert.ToInt64(request.batchId);

                        Batch batcchtounlock = exorabilisContext.Batch.Where(x => x.Id == batchId).FirstOrDefault();

                        if (batcchtounlock != null)
                        {
                            batcchtounlock.IsLocked = null;
                            batcchtounlock.LockedBy = null;
                        }
                        break;

                    case 7:  // export all batch
                        BatchSearchRequest searchRequest = new BatchSearchRequest
                        {
                            batchnumber = request.batchnumber,
                            webhostEnvironment = request.webhostEnvironment,
                            firstdate = request.firstdate,
                            lastdate = request.lastdate
                        };
                        Export(searchRequest);
                        break;
                    case 5: // unlock one batch



                        var datatounlock = from b in exorabilisContext.Batch
                                           join status in exorabilisContext.DocumentStatus on b.DocumentStatusId equals status.Id
                                           join step in exorabilisContext.DocumentStep on b.DocumentStepId equals step.Id
                                           join type in exorabilisContext.DocumentType on b.DocumentStatusId equals type.Id
                                           join reject in exorabilisContext.RejectionCode on b.RejectionCodeId equals reject.Id into rejected
                                           from reject in rejected.DefaultIfEmpty()
                                           join file in exorabilisContext.Files on b.Id equals file.BatchId // assuming you want to join with File for the FileType
                                           select b;

                        if (request.firstdate != null)
                        {

                            datatounlock = from d in datatounlock
                                           where (d.LockedOn.HasValue && DateOnly.FromDateTime(d.LockedOn.Value) >= request.firstdate) || (d.SanityOn.HasValue && DateOnly.FromDateTime(d.SanityOn.Value) >= request.firstdate) || (d.QualityOn.HasValue && DateOnly.FromDateTime(d.QualityOn.Value) >= request.firstdate) || (d.IndexedOn.HasValue && DateOnly.FromDateTime(d.IndexedOn.Value) >= request.firstdate) || (d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.ReviewedOn.Value) >= request.firstdate) || (d.ScannedOn.HasValue && DateOnly.FromDateTime(d.ScannedOn.Value) >= request.firstdate)
                                           select d;
                        }

                        if (request.lastdate != null)
                        {

                            datatounlock = from d in datatounlock
                                           where (d.LockedOn.HasValue && DateOnly.FromDateTime(d.LockedOn.Value) <= request.lastdate) || (d.SanityOn.HasValue && DateOnly.FromDateTime(d.SanityOn.Value) <= request.lastdate) || (d.QualityOn.HasValue && DateOnly.FromDateTime(d.QualityOn.Value) <= request.lastdate) || (d.IndexedOn.HasValue && DateOnly.FromDateTime(d.IndexedOn.Value) <= request.lastdate) || (d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.ReviewedOn.Value) <= request.lastdate) || (d.ScannedOn.HasValue && DateOnly.FromDateTime(d.ScannedOn.Value) <= request.lastdate)
                                           select d;
                        }

                        if (request.batchnumber != null)
                        {
                            datatounlock = from d in datatounlock
                                           where d.ReferenceNumber.Contains(request.batchnumber)
                                           select d;
                        }



                        var datatounlocker = await (from d in datatounlock
                                                    where d.DocumentStepId != 1
                                     && d.IsLocked == 1
                                       && !d.RejectionCodeId.HasValue
                                       && d.NumberOfDocument > 0
                                                    select d).Distinct().ToListAsync();


                        foreach (var item in datatounlocker)
                        {
                            item.IsLocked = null;
                            item.LockedBy = null;
                        }
                        break;
                    default:
                        break;
                }



                await this.exorabilisContext.SaveChangesAsync();

                //transaction.Commit();

                return response;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }

        }

        public void createOrUpdateBatchHistory(Batch batch, string userId, string otherreason)
        {
            try
            {

                var batchHisto = new BatchHistory();
                batchHisto.BatchId = batch.Id;
                batchHisto.DocumentStatusId = batch.DocumentStatusId;
                batchHisto.DocumentStepId = batch.DocumentStepId;
                batchHisto.NumberOfDocument = batch.NumberOfDocument;
                batchHisto.RejectionCode = batch.RejectionCodeId;
                batchHisto.UserId = userId;
                batchHisto.StartedOn = GlobalVariables.StartDate;
                batchHisto.EndedOn = DateTime.Now;
                batchHisto.LastUpdatedOn = DateTime.Now;
                batchHisto.Remark = otherreason;

                exorabilisContext.BatchHistory.AddAsync(batchHisto);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Batch SetActionDoneByBatch(Batch document, string userName)
        {
            if (document != null)
            {
                switch (document.DocumentStepId)
                {
                    case 2:
                        if (string.IsNullOrEmpty(document.QualityBy))
                        {

                            document.QualityBy = userName;
                            document.QualityOn = DateTime.Now;
                            document.LastStep = "Quality";
                        }

                        document.IndexedBy = userName;
                        document.IndexedOn = DateTime.Now;
                        document.LastStep = "Indexing";

                        break;
                    case 3:
                        document.IndexedBy = userName;
                        document.IndexedOn = DateTime.Now;
                        document.LastStep = "Indexing";
                        break;

                    case 5:
                        document.SanityBy = userName;
                        document.SanityOn = DateTime.Now;
                        document.LastStep = "Sanity";
                        break;

                    case 8:
                        document.ReviewedBy = userName;
                        document.ReviewedOn = DateTime.Now;
                        document.LastStep = "Reviewed";
                        break;
                }
            }

            return document;
        }

        public async Task<bool> RejectBatch(string batchId, long rejectionId, string UserId, string UserName, string otherreason)
        {
            var transaction = this.exorabilisContext.Database.BeginTransaction();

            try
            {
                GlobalVariables.StartDate = DateTime.Now;

                var response = true;
                var data = Convert.ToInt64(batchId);

                Batch batch = await exorabilisContext.Batch.Where(x => x.Id == data).FirstOrDefaultAsync();

                if (batch != null)
                {

                    this.SetActionDoneByBatch(batch, UserName);

                    batch.QualityBy = UserName;
                    batch.QualityOn = DateTime.Now;

                    batch.IsLocked = null;
                    batch.LockedBy = null;
                    batch.DocumentStatusId = 5;
                    batch.RejectionCodeId = rejectionId;
                    batch.ReasonOther = otherreason;

                    (from d in exorabilisContext.Document
                     join f in exorabilisContext.Files on d.FileId equals f.Id
                     where f.BatchId == data
                     select d).ToList().ForEach(x =>
                     {
                         x.RejectionCodeId = rejectionId;
                         x.DocumentStatusId = 5;
                         x.RescanBy = UserName;
                         x.RescanOn = DateTime.Now;

                     });

                    var folderToTransfert = GetValueParam("FolderToTransfert").Value;

                    createOrUpdateBatchHistory(batch, UserId.ToString(), otherreason);

                    //var oxiPath = Path.Combine(folderToTransfert?.Value?.ToString(), batch?.ReferenceNumber);

                    //if (Directory.Exists(oxiPath)) { DeleteDirectory(oxiPath); }

                }

                await this.exorabilisContext.SaveChangesAsync();

                transaction.Commit();

                return response;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }

        }
        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        public async Task<IEnumerable<RejectionCode>> getAllRejectionCode()
        {
            try
            {

                var resultat = await this.exorabilisContext.RejectionCode.ToListAsync();



                return resultat;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task LockBatch(string batchId, string username)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();

            try
            {
                var data = batchId;
                Batch batch = await this.exorabilisContext.Batch.Where(x => x.Id == Convert.ToInt64(data)).FirstAsync(); // leve une exception si aucun resultat firstAsync

                batch.IsLocked = 1;
                batch.LockedBy = username;
                batch.LockedOn = DateTime.Now;
                await exorabilisContext.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }
    }
}
