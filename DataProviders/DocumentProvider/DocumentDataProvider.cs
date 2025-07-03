using Core.Database;
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.Entity.MyCore;
using Core.RequestPostGet;
using Core.ServiceEncryptor;
using Core.ViewModel.Exorabilis;
using DataProviders.BatchProvider;
using DocumentFormat.OpenXml.Spreadsheet;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static GdPicture.Internal.MSOfficeBinary.translator.OfficeGraph.BOF;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Org.BouncyCastle.Utilities.Test.FixedSecureRandom;
using static SkiaSharp.HarfBuzz.SKShaper;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Image = Core.Entity.Exorabilis.Image;

namespace DataProviders.DocumentProvider
{
    public class DocumentDataProvider : InterfaceDocumentDataProvider
    {
        private readonly ExorabilisContext exorabilisContext;
        private readonly InterfaceBatchDataProvider interfaceBatchDataProvider;

        public DocumentDataProvider(ExorabilisContext exorabilis, InterfaceBatchDataProvider interfaceBatchData)
        {
            this.exorabilisContext = exorabilis;
            this.interfaceBatchDataProvider = interfaceBatchData;

        }


        /// <summary>
        ///  get all documents if we click on batch detail 
        ///     if step == 12 ( batch to rescan ) : we get the docs corresponding on btahc who has a status 5 or 6 ( completed to scan )
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsByBatchWithStep(BatchwithStepRequest batchwithStep)
        {
            try
            {

                var statusall = new List<long?> { 4, 2, 5 };
                var stepall = new List<long?> { batchwithStep.step, 8 };
                var doctype = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                if (batchwithStep.step == 5) // 
                {

                    var query1 = from f in exorabilisContext.Files
                                 join d in exorabilisContext.Document on f.Id equals d.FileId
                                 join b in exorabilisContext.Batch on f.BatchId equals b.Id
                                 join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                                 join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id into statusfinal
                                 from status in statusfinal.DefaultIfEmpty()
                                 join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id into stepFinal
                                 from step in stepFinal.DefaultIfEmpty()
                                 join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id into typefinal
                                 from type in typefinal.DefaultIfEmpty()
                                 join code in exorabilisContext.RejectionCode on d.RejectionCodeId equals code.Id into reject
                                 from rejcode in reject.DefaultIfEmpty()
                                 where ((d.DocumentStepId == 5 && d.DocumentStatusId == 4) || (d.DocumentStepId == 8 && d.DocumentStatusId == 2) || (d.DocumentStepId == 8 && d.DocumentStatusId == 5)) &&
                                       f.BatchId == Convert.ToInt64(batchwithStep.batchId) &&
                                       ir.Status == 1 &&
                                       d.DocumentTypeId == Convert.ToInt64(doctype)
                                 select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                                 {
                                     ImageId = ir.Id,
                                     IdEncrypt = d.Id.ToString(),
                                     FileId = f.Id,
                                     BatchNumber = b.ReferenceNumber,
                                     DocumentNumber = d.ReferenceNumber,
                                     DocumentTypeName = type.Name,
                                     DocumentStatusName = status.Name,
                                     DocumentStatusId = status.Id,
                                     DocumentStepName = step.Name,
                                     RejectionCodeName = rejcode.Name,
                                     CroppedOn = d.CroppedOn,
                                     PageOrder = d.PageOrder,
                                     LastUpdatedOn = d.LastUpdatedOn,
                                     IndexedOn = d.IndexedOn,
                                     ReasonOther = d.ReasonOther,
                                     BatchId = d.BatchId,
                                     CreatedBy = d.CreatedBy,
                                     LastStep = d.LastStep,
                                     ModifiedBy = d.ModifiedBy,
                                     ModifiedOn = d.ModifiedOn,
                                     QualityBy = d.QualityBy,
                                     QualityOn = d.QualityOn,
                                     Pcname = d.Pcname,
                                     RescanBy = d.RescanBy,
                                     RescanOn = d.RescanOn,
                                     RejectedBy = d.RejectedBy,
                                     RejectedOn = d.RejectedOn,

                                     IndexedBy = d.IndexedBy,
                                     CroppedBy = d.CroppedBy,
                                     SanityOn = d.SanityOn,
                                     SanityBy = d.SanityBy,
                                     FinalQualityOn = d.FinalQualityOn,
                                     FinalQualityBy = d.FinalQualityBy,
                                     ReviewedOn = d.ReviewedOn,
                                     ReviewedBy = d.ReviewedBy,
                                     CreatedOn = d.CreatedOn,
                                     FileTypeId = b.File.FileTypeId
                                     //IndexModels = allindex

                                 };



                    var result1 = query1.Distinct().ToList();

                    var groupedResult1 = result1
                     .GroupBy(x => x.FileId)
                     .SelectMany(group =>
                     {
                         var grouped = group.First();
                         var isSingle = group.First().FileTypeId == 1;
                         var NumberOfPages = (from img in exorabilisContext.Image
                                              join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                              where img.DocumentTypeId == Convert.ToInt64(doctype) && docx.FileId == grouped.FileId && img.Status == 1
                                              select img
                                             ).Count();
                         var NumberOfDocument = Convert.ToInt32(NumberOfPages / 2);
                         int count = NumberOfDocument;

                         var documents = isSingle ? group : group.Take(1);

                         foreach (var doc in documents)
                         {
                             var folios = (from file in exorabilisContext.Files
                                           join pfolio in exorabilisContext.Project_folio on file.Project_folio_id equals pfolio.Id
                                           where file.Id == doc.FileId
                                           select pfolio).FirstOrDefault();
                             doc.pagesCount = NumberOfPages;
                             doc.GroupCount = count; // Affectation directe
                             doc.Folio = folios == null ? "" : $"{folios.Folios} ( {folios.NbrOfPages} )";
                         }

                         return documents;
                     })
                     .OrderBy(i => i.PageOrder)
                     .ToList();


                    return groupedResult1;

                }
                if (batchwithStep.step == 12) // rescan
                {
                    statusall = new List<long?> { 5, 6 };
                    stepall = new List<long?> { 2, 3, 5, 8 };
                }
                if (batchwithStep.step == 11) // unlock
                {
                    statusall = new List<long?> { 1, 2, 4 };
                    stepall = new List<long?> { 2, 3, 5, 8 };
                }
                if (batchwithStep.step == 14) //
                {
                    statusall = new List<long?> { 1 };
                    stepall = new List<long?> { 8 };
                }

                var query = from d in exorabilisContext.Document
                            join f in exorabilisContext.Files on d.FileId equals f.Id
                            join b in exorabilisContext.Batch on f.BatchId equals b.Id
                            join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                            join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id into statusfinal
                            from status in statusfinal.DefaultIfEmpty()
                            join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id into stepFinal
                            from step in stepFinal.DefaultIfEmpty()
                            join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id into typefinal
                            from type in typefinal.DefaultIfEmpty()
                            join code in exorabilisContext.RejectionCode on d.RejectionCodeId equals code.Id into reject
                            from rejcode in reject.DefaultIfEmpty()
                            where stepall.Contains(d.DocumentStepId) && ir.Status == 1 &&
                               statusall.Contains(d.DocumentStatusId) &&
                                  f.BatchId == Convert.ToInt64(batchwithStep.batchId) &&
                                  //d.RejectionCodeId == null &&
                                  d.DocumentTypeId == Convert.ToInt64(doctype)
                            select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                            {
                                ImageId = ir.Id,
                                IdEncrypt = d.Id.ToString(),
                                FileId = f.Id,
                                BatchNumber = b.ReferenceNumber,
                                DocumentNumber = d.ReferenceNumber,
                                DocumentTypeName = type.Name,
                                DocumentStatusName = status.Name,
                                DocumentStatusId = status.Id,
                                DocumentStepName = step.Name,
                                RejectionCodeName = rejcode.Name,
                                CroppedOn = d.CroppedOn,
                                PageOrder = d.PageOrder,
                                LastUpdatedOn = d.LastUpdatedOn,
                                IndexedOn = d.IndexedOn,
                                ReasonOther = d.ReasonOther,
                                BatchId = d.BatchId,
                                CreatedBy = d.CreatedBy,
                                LastStep = d.LastStep,
                                ModifiedBy = d.ModifiedBy,
                                ModifiedOn = d.ModifiedOn,
                                QualityBy = d.QualityBy,
                                QualityOn = d.QualityOn,
                                Pcname = d.Pcname,
                                RescanBy = d.RescanBy,
                                RescanOn = d.RescanOn,
                                RejectedBy = d.RejectedBy,
                                RejectedOn = d.RejectedOn,

                                IndexedBy = d.IndexedBy,
                                CroppedBy = d.CroppedBy,
                                SanityOn = d.SanityOn,
                                SanityBy = d.SanityBy,
                                FinalQualityOn = d.FinalQualityOn,
                                FinalQualityBy = d.FinalQualityBy,
                                ReviewedOn = d.ReviewedOn,
                                ReviewedBy = d.ReviewedBy,
                                CreatedOn = d.CreatedOn,
                                FileTypeId = b.File.FileTypeId
                                //IndexModels = allindex

                            };



                var result = query.Distinct().ToList();

                var groupedResult = result
                 .GroupBy(x => x.FileId)
                 .SelectMany(group =>
                 {
                     var grouped = group.First();
                     var isSingle = group.First().FileTypeId == 1;
                     var NumberOfPages = 0;
                     if (batchwithStep.step == 12) // rescan
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctype) && docx.FileId == grouped.FileId && img.Status == 1
                                          select img
                                         ).Count();
                     }
                     else
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctype) && docx.FileId == grouped.FileId && img.Status == 1 && (docx.DocumentStatusId == 1 || docx.DocumentStatusId == 4)
                                          select img
                                         ).Count();
                     }


                     var NumberOfDocument = Convert.ToInt32(NumberOfPages / 2);
                     int count = NumberOfDocument;

                     var documents = isSingle ? group : group.Take(1);

                     foreach (var doc in documents)
                     {
                         var folios = (from file in exorabilisContext.Files
                                       join pfolio in exorabilisContext.Project_folio on file.Project_folio_id equals pfolio.Id
                                       where file.Id == doc.FileId
                                       select pfolio).FirstOrDefault();
                         doc.pagesCount = NumberOfPages;
                         doc.GroupCount = count; // Affectation directe
                         doc.Folio = folios == null ? "" : $"{folios.Folios} ( {folios.NbrOfPages} )";
                     }

                     return documents;
                 })
                 .OrderBy(i => i.PageOrder)
                 .ToList();

                return groupedResult;


            }
            catch (Exception ex)
            {

                throw;
            }

        }
        /// <summary>
        ///  get all documents if we click on batch detail 
        ///     if step == 12 ( batch to rescan ) : we get the docs corresponding on btahc who has a status 5 or 6 ( completed to scan )
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsByBatchToOrder(BatchwithStepRequest batchwithStep)
        {
            try
            {

                var statusall = new List<long?> { 4, 2 };
                var stepall = new List<long?> { batchwithStep.step, 8 };
                var doctype = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                statusall = new List<long?> { 1, 4 };
                stepall = new List<long?> { 2, 3, 5, 8 };


                var query = from f in exorabilisContext.Files
                            join d in exorabilisContext.Document on f.Id equals d.FileId
                            join b in exorabilisContext.Batch on f.BatchId equals b.Id
                            join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                            join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id into statusfinal
                            from status in statusfinal.DefaultIfEmpty()
                            join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id into stepFinal
                            from step in stepFinal.DefaultIfEmpty()
                            join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id into typefinal
                            from type in typefinal.DefaultIfEmpty()
                            join code in exorabilisContext.RejectionCode on d.RejectionCodeId equals code.Id into reject
                            from rejcode in reject.DefaultIfEmpty()
                            where stepall.Contains(d.DocumentStepId) &&
                               statusall.Contains(d.DocumentStatusId) && ir.Status == 1 &&
                                  f.BatchId == Convert.ToInt64(batchwithStep.batchId) &&
                                  //d.RejectionCodeId == null &&
                                  (d.DocumentTypeId == Convert.ToInt64(doctype) || d.DocumentTypeId == 7 )
                            select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                            {
                                ProjectReference = b.FileNumber,
                                ImageId = ir.Id,
                                BatchId = d.BatchId,
                                IdEncrypt = d.Id.ToString(),
                                FileId = f.Id,
                                BatchNumber = b.ReferenceNumber,
                                DocumentNumber = d.ReferenceNumber,
                                DocumentTypeName = type.Name,
                                DocumentStatusName = status.Name,
                                DocumentStepName = step.Name,
                                CroppedOn = d.CroppedOn,
                                PageOrder = d.PageOrder,
                                LastUpdatedOn = d.LastUpdatedOn,
                                IndexedOn = d.IndexedOn,
                                ReasonOther = d.ReasonOther,
                                Pcname = d.Pcname,
                                RejectionCodeName = rejcode.Name,
                                CreatedBy = d.CreatedBy,
                                LastStep = d.LastStep,
                                ModifiedBy = d.ModifiedBy,
                                ModifiedOn = d.ModifiedOn,
                                QualityBy = d.QualityBy,
                                QualityOn = d.QualityOn,

                                RescanBy = d.RescanBy,
                                RescanOn = d.RescanOn,
                                RejectedBy = d.RejectedBy,
                                RejectedOn = d.RejectedOn,

                                IndexedBy = d.IndexedBy,
                                CroppedBy = d.CroppedBy,
                                SanityOn = d.SanityOn,
                                SanityBy = d.SanityBy,
                                FinalQualityOn = d.FinalQualityOn,
                                FinalQualityBy = d.FinalQualityBy,
                                ReviewedOn = d.ReviewedOn,
                                ReviewedBy = d.ReviewedBy,
                                CreatedOn = d.CreatedOn,
                                FileTypeId = b.File.FileTypeId
                                //IndexModels = allindex

                            };



                var result = query.Distinct().ToList();

                var groupedResult = result
                 .GroupBy(x => x.FileId)
                 .SelectMany(group =>
                 {
                     var grouped = group.First();
                     var isSingle = group.First().FileTypeId == 1;
                     var NumberOfPages = 0;
                     if (batchwithStep.step == 12) // rescan unlock
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctype) && docx.FileId == grouped.FileId && img.Status == 1
                                          select img
                                         ).Count();
                     }
                     else
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctype) && docx.FileId == grouped.FileId && img.Status == 1 && (docx.DocumentStatusId == 1 || docx.DocumentStatusId == 4)
                                          select img
                                         ).Count();
                     }
                     var NumberOfDocument = Convert.ToInt32(NumberOfPages / 2);
                     int count = NumberOfDocument;

                     var documents = isSingle ? group : group.Take(1);

                     foreach (var doc in documents)
                     {
                         var folios = (from file in exorabilisContext.Files
                                       join pfolio in exorabilisContext.Project_folio on file.Project_folio_id equals pfolio.Id
                                       where file.Id == doc.FileId
                                       select pfolio).FirstOrDefault();
                         doc.pagesCount = NumberOfPages;
                         doc.GroupCount = count; // Affectation directe
                         doc.Folio = folios == null ? "" : $"{folios.Folios} ( {folios.NbrOfPages} )";
                     }

                     return documents;
                 })
                 .OrderBy(i => i.PageOrder)
                 .ToList();


                return groupedResult;


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// get all document review, search , rescan ( step 5 : because the status change 5 if supervisuer reject in menu "saim review" but the batch status is not changed ) , modification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsReview(DocumentSearchRequest request)
        {
            try
            {


                var reqstep = Convert.ToInt64(request.step.ToString());

                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                var docstatus = new List<long> { };
                var docstep = new List<long> { };
                var doctype = new List<long> { };

                if (reqstep == 8) // review
                {
                    docstatus = new List<long> { 4, 2 };
                    docstep = new List<long> { 8 };
                    doctype = new List<long> { Convert.ToInt64(doctypeid),7 };
                }

                if (reqstep == 9) // search
                {
                    docstatus = new List<long> { 1 };
                    docstep = new List<long> { 8 };
                    doctype = new List<long> { Convert.ToInt64(doctypeid ),7 };
                }

                if (reqstep == 10) // modification
                {
                    docstatus = new List<long> { 4, 1 };
                    docstep = new List<long> { 5, 8 };
                    doctype = new List<long> { Convert.ToInt64(doctypeid ) ,7};
                }

                if (reqstep == 13) // document rescan
                {
                    docstatus = new List<long> { 5, 6 };
                    docstep = new List<long> { 5, 8, 2, 3 };
                    doctype = new List<long> { Convert.ToInt64(doctypeid ),7 };
                }

                var data = (from d in exorabilisContext.Document
                            join f in exorabilisContext.Files on d.FileId equals f.Id
                            join ftype in exorabilisContext.FileType on f.FileTypeId equals ftype.Id
                            join b in exorabilisContext.Batch on f.BatchId equals b.Id
                            join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                            join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id
                            join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id
                            join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id
                            join code in exorabilisContext.RejectionCode on d.RejectionCodeId equals code.Id into reject
                            from rejcode in reject.DefaultIfEmpty()
                            where ir.Status == 1 && (b.DocumentStatusId != 5 && b.DocumentStatusId != 6) && docstep.Contains(d.DocumentStepId.Value) && docstatus.Contains(d.DocumentStatusId.Value) && doctype.Contains(d.DocumentTypeId.Value)

                            select new
                            {
                                f,
                                ftype,
                                d,
                                b,
                                status,
                                step,
                                type,
                                rejcode
                            });

                if (request.firstdate != null)
                {
                    if (reqstep == 10) // modification
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                               (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) >= request.firstdate) ||
                               (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) >= request.firstdate) ||
                               (d.d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.d.ReviewedOn.Value) >= request.firstdate) ||
                               (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) >= request.firstdate) ||
                               (d.d.ModifiedOn.HasValue && DateOnly.FromDateTime(d.d.ModifiedOn.Value) >= request.firstdate)
                               select d;
                    }
                    else if (reqstep == 9) // search
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                               (d.d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.d.ReviewedOn.Value) >= request.firstdate) ||
                               (d.d.ModifiedOn.HasValue && DateOnly.FromDateTime(d.d.ModifiedOn.Value) >= request.firstdate)
                               select d;
                    }
                    else if (reqstep == 13) // document rescan
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                               (d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate)
                               select d;
                    }
                    else
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                               (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) >= request.firstdate) ||
                               (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) >= request.firstdate)
                               select d;
                    }

                }

                if (request.lastdate != null)
                {
                    if (reqstep == 10) // modification
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                               (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) <= request.lastdate) ||
                               (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate) ||
                               (d.d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.d.ReviewedOn.Value) <= request.lastdate) ||
                               (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) <= request.lastdate) ||
                               (d.d.ModifiedOn.HasValue && DateOnly.FromDateTime(d.d.ModifiedOn.Value) <= request.lastdate)
                               select d;
                    }
                    else if (reqstep == 9) // search
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                               (d.d.ReviewedOn.HasValue && DateOnly.FromDateTime(d.d.ReviewedOn.Value) <= request.lastdate) ||
                               (d.d.ModifiedOn.HasValue && DateOnly.FromDateTime(d.d.ModifiedOn.Value) <= request.lastdate)
                               select d;
                    }
                    else if (reqstep == 13) // document rescan
                    {
                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                               (d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate)
                               select d;
                    }
                    else
                    {

                        data = from d in data
                               where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                                (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) <= request.lastdate) ||
                                (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) <= request.lastdate)
                               select d;
                    }
                }

                if (request.batchnumber != null)
                {
                    data = from d in data
                           where d.b.ReferenceNumber.Contains(request.batchnumber)
                           select d;
                }

                if (request.documentnumber != null)
                {
                    data = from d in data
                           where d.d.ReferenceNumber.Contains(request.documentnumber)
                           select d;
                }

                var result = (from d in data
                              select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                              {
                                  ProjectReference = d.b.FileNumber,
                                  Id = d.d.Id,
                                  IdEncrypt = d.d.Id.ToString(),
                                  FileId = d.f.Id,
                                  BatchNumber = d.b.ReferenceNumber,
                                  DocumentNumber = d.d.ReferenceNumber,
                                  DocumentTypeName = d.type.Name,
                                  DocumentStatusName = d.status.Name,
                                  DocumentStepName = d.step.Name,
                                  DocumentStatusId = d.status.Id,
                                  RejectionCodeName = d.rejcode.Name,
                                  CroppedOn = d.d.CroppedOn,
                                  PageOrder = d.d.PageOrder,
                                  LastUpdatedOn = d.d.LastUpdatedOn,
                                  IndexedOn = d.d.IndexedOn,
                                  ReasonOther = d.d.ReasonOther,
                                  IndexedBy = d.d.IndexedBy,
                                  CroppedBy = d.d.CroppedBy,
                                  SanityOn = d.d.SanityOn,
                                  SanityBy = d.d.SanityBy,
                                  FinalQualityOn = d.d.FinalQualityOn,
                                  FinalQualityBy = d.d.FinalQualityBy,
                                  ReviewedOn = d.d.ReviewedOn,
                                  ReviewedBy = d.d.ReviewedBy,
                                  Pcname = d.d.Pcname,

                                  RescanBy = d.d.RescanBy,
                                  RescanOn = d.d.RescanOn,
                                  RejectedBy = d.d.RejectedBy,
                                  RejectedOn = d.d.RejectedOn,

                                  CreatedBy = d.d.CreatedBy,
                                  LastStep = d.d.LastStep,
                                  ModifiedBy = d.d.ModifiedBy,
                                  ModifiedOn = d.d.ModifiedOn,
                                  QualityBy = d.d.QualityBy,
                                  QualityOn = d.d.QualityOn,
                                  ExportStatus = d.d.ExportStatus,
                                  CreatedOn = d.d.CreatedOn,
                                  FileTypeId = d.b.File.FileTypeId,
                                  FileTypeName = d.ftype.Name,
                                  IndexModels = exorabilisContext.DocumentIndex.Where(x => x.DocumentId == d.d.Id).ToList()

                              }).ToList().DistinctBy(x => x.Id);




                var groupedResult = result
                 .GroupBy(x => x.FileId)
                 .SelectMany(group =>
                 {
                     var grouped = group.First();
                     var isSingle = group.First().FileTypeId == 1;
                     var NumberOfPages = 0;
                     if (reqstep == 12) // rescan
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctypeid) && docx.FileId == grouped.FileId && img.Status == 1
                                          select img
                                         ).Count();
                     }
                     else
                     {
                         NumberOfPages = (from img in exorabilisContext.Image
                                          join docx in exorabilisContext.Document on img.DocumentId equals docx.Id
                                          where img.DocumentTypeId == Convert.ToInt64(doctypeid) && docx.FileId == grouped.FileId && img.Status == 1 && (docx.DocumentStatusId == 1 || docx.DocumentStatusId == 4)
                                          select img
                                         ).Count();
                     }
                     var NumberOfDocument = Convert.ToInt32(NumberOfPages / 2);
                     int count = NumberOfDocument;

                     var documents = isSingle ? group : group.Take(1);

                     foreach (var doc in documents)
                     {
                         var folios = (from file in exorabilisContext.Files
                                        join pfolio in exorabilisContext.Project_folio on file.Project_folio_id equals pfolio.Id
                                        where file.Id == doc.FileId
                                        select pfolio).FirstOrDefault();
                         doc.pagesCount = NumberOfPages;
                         doc.GroupCount = count; // Affectation directe
                         doc.Folio = folios == null ? "" : $"{folios.Folios} ( {folios.NbrOfPages} )";
                     }

                     return documents;
                 })
                 .OrderBy(i => i.PageOrder)
                 .ToList();


                return groupedResult;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<Dashboard_DTO> Dashboard(DocumentSearchRequest request)
        {
            try
            {
                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;
                var showTaskByUser = interfaceBatchDataProvider.GetValueParam("showTaskByUser").Value;

                var doctype = new List<long> { Convert.ToInt64(doctypeid) };


                // to be qualited
                var scanforupdate = (from d in exorabilisContext.Document
                                     join b in exorabilisContext.Batch on d.BatchId equals b.Id
                                     join f in exorabilisContext.Files on d.FileId equals f.Id
                                     join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                     where doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue && d.ExportStatus == 1
                                     select new { d, b });

                foreach (var item in scanforupdate)
                {
                    item.d.ExportOn = item.d.QualityOn;
                    item.b.ExportOn = item.d.QualityOn;
                }
                exorabilisContext.SaveChanges();


                // scan 
                var scantotal = (from d in exorabilisContext.Document
                                 join f in exorabilisContext.Files on d.FileId equals f.Id
                                 join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                 where (d.DocumentStatusId != 5 && d.DocumentStatusId != 6 && d.DocumentStatusId != 2) && doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue && Image.Status == 1
                                 select new { d, f, Image });

                // to be qualited
                var scannedtotal = (from d in exorabilisContext.Document
                                    join f in exorabilisContext.Files on d.FileId equals f.Id
                                    join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                    where (d.DocumentStatusId != 5 && d.DocumentStatusId != 6 && d.DocumentStatusId != 2) && d.DocumentStepId == 2 && doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue && Image.Status == 1
                                    select new { d, f, Image });
                //qualited
                var qualitytotal = (from d in exorabilisContext.Document
                                    join f in exorabilisContext.Files on d.FileId equals f.Id
                                    join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                    where (d.DocumentStatusId != 5 && d.DocumentStatusId != 6 && d.DocumentStatusId != 2) && d.DocumentStatusId == 1 && d.DocumentStepId == 8 && doctype.Contains(d.DocumentTypeId.Value) && Image.Status == 1
                                    && ((d.QualityOn.HasValue))
                                    select new { d, f, Image });

                // to be export
                var completedtotal = (from d in exorabilisContext.Document
                                      join f in exorabilisContext.Files on d.FileId equals f.Id
                                      join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                      where d.DocumentStatusId == 1 && d.QualityOn.HasValue && d.DocumentStepId == 8 && d.ExportStatus == 0 && doctype.Contains(d.DocumentTypeId.Value) && Image.Status == 1
                                      select new { d, f, Image });

                // exported
                var completedtotalexported = (from d in exorabilisContext.Document
                                              join f in exorabilisContext.Files on d.FileId equals f.Id
                                              join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                              where d.DocumentStatusId == 1 && d.ExportOn.HasValue && d.DocumentStepId == 8 && d.ExportStatus == 1 && doctype.Contains(d.DocumentTypeId.Value) && Image.Status == 1
                                              select new { d, f, Image });


                var rescantotal = (from d in exorabilisContext.Document
                                   join f in exorabilisContext.Files on d.FileId equals f.Id
                                   join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                   where (d.DocumentStatusId == 5) && doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue && Image.Status == 1
                                   select new { d, f, Image });

                var rescancompletedtotal = (from d in exorabilisContext.Document
                                            join f in exorabilisContext.Files on d.FileId equals f.Id
                                            join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                            where (d.DocumentStatusId == 6) && doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue && Image.Status == 1
                                            select new { d, f, Image });


                if (request.firstdate != null)
                {
                    //// to be quality
                    //scannedtotal = from d in scannedtotal
                    //               where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
                    //               select d;

                    // scan
                    scantotal = from d in scantotal
                                where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
                                select d;



                    // qualitytotalfini
                    qualitytotal = from d in qualitytotal
                                   where
                                  (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) >= request.firstdate)
                                   select d;


                    // to be rescan
                    rescantotal = from d in rescantotal
                                  where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
                                  select d;



                    rescancompletedtotal = from d in rescancompletedtotal
                                           where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) >= request.firstdate
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


                    // to be index
                    qualitytotal = from d in qualitytotal
                                   where
                                   (d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate)
                                   select d;


                    // to be rescan
                    rescantotal = from d in rescantotal
                                  where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                  select d;


                    rescancompletedtotal = from d in rescancompletedtotal
                                           where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                           select d;


                    // exported  TODO , si c'a ete passer par saim review : ca aura ete ReviewedOn
                    completedtotalexported = from d in completedtotalexported
                                             where d.d.ExportOn.HasValue && DateOnly.FromDateTime(d.d.ExportOn.Value) <= request.lastdate
                                             select d;

                }
                // for the users

                var finalList = new List<UserCount>();

                #region users
                // for the users

                if (showTaskByUser.ToString() == "1")
                {
                    var scantotalbyuser = (from d in exorabilisContext.Document
                                           join f in exorabilisContext.Files on d.FileId equals f.Id
                                           join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                           where (d.DocumentStepId > 1) && doctype.Contains(d.DocumentTypeId.Value) && d.CreatedOn.HasValue && Image.Status == 1
                                           select new { d, f, Image });


                    var qualitytotalbyuser = (from d in exorabilisContext.Document
                                              join f in exorabilisContext.Files on d.FileId equals f.Id
                                              join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                              where doctype.Contains(d.DocumentTypeId.Value) && d.QualityOn.HasValue && Image.Status == 1
                                              select new { d, f, Image });



                    var completedtotalbyuser = (from d in exorabilisContext.Document
                                                join f in exorabilisContext.Files on d.FileId equals f.Id
                                                join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                                where d.DocumentStatusId == 1 && d.ReviewedOn.HasValue && d.DocumentStepId == 8 && doctype.Contains(d.DocumentTypeId.Value) && Image.Status == 1
                                                select new { d, f, Image });

                    var rescantotalbyuser = (from d in exorabilisContext.Document
                                             join f in exorabilisContext.Files on d.FileId equals f.Id
                                             join Image in exorabilisContext.Image on d.Id equals Image.DocumentId
                                             where doctype.Contains(d.DocumentTypeId.Value) && d.RescanOn.HasValue && Image.Status == 1
                                             select new { d, f, Image });


                    if (request.firstdate != null)
                    {

                        // scan by user
                        scantotalbyuser = from d in scantotalbyuser
                                          where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate
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

                    }

                    if (request.lastdate != null)
                    {
                        // scan by user
                        scantotalbyuser = from d in scantotalbyuser
                                          where d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate
                                          select d;

                        // quality by user 
                        qualitytotalbyuser = from d in qualitytotalbyuser
                                             where d.d.QualityOn.HasValue && DateOnly.FromDateTime(d.d.QualityOn.Value) <= request.lastdate
                                             select d;


                        // send to rescan by user
                        rescantotalbyuser = from d in rescantotalbyuser
                                            where d.d.RescanOn.HasValue && DateOnly.FromDateTime(d.d.RescanOn.Value) <= request.lastdate
                                            select d;


                    }


                    // scan complete

                    var scantotalbyuser1 = scantotalbyuser.ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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

                    var scantotalfinalebyuser = (from d in scantotalbyuser1
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

                    // complete reviewed


                    var completedtotalbyuser1 = completedtotalbyuser.ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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

                    var completedtotalfinalbyuser = (from d in completedtotalbyuser1
                                                     group d by d.d.ReviewedBy into g
                                                     select new
                                                     {
                                                         Username = g.Key,
                                                         count = g.Count()
                                                     }).ToList();


                    var allUsers = scantotalfinalebyuser.Select(x => x.Username)
                    .Union(qualitytotalfinalebyuser.Select(x => x.Username))
                    .Union(rescantotalfinalebyuser.Select(x => x.Username))
                    .Union(completedtotalfinalbyuser.Select(x => x.Username))
                    .Distinct();

                    // Fusionner les données par utilisateur
                    finalList = allUsers.Select(username => new UserCount
                    {
                        Username = username,
                        Scan = scantotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0,
                        Quality = qualitytotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0,
                        Rescan = rescantotalfinalebyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0,
                        Completed = completedtotalfinalbyuser.FirstOrDefault(x => x.Username == username)?.count ?? 0
                    }).ToList();

                }
                #endregion
                //sheets
                var scannedtotalfinalesheets = (from d in scannedtotal select d).ToList().DistinctBy(x => x.Image.Id);
                //documents // to be quality
                var scannedtotalfinale = (from d in scannedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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
                var scantotalfinalesheets = (from d in scantotal select d).ToList().DistinctBy(x => x.Image.Id);
                //documents // scan
                var scantotalfinale = (from d in scantotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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
                var qualitytotalfinalesheets = (from d in qualitytotal select d).ToList().DistinctBy(x => x.Image.Id);
                // to be indexed
                var qualitytotalfinale = (from d in qualitytotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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



                var rescantotalfinalesheets = (from d in rescantotal select d).ToList().DistinctBy(x => x.Image.Id);
                var rescantotalfinale = (from d in rescantotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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
                var completedtotalfinalesheets = (from d in completedtotal select d).ToList().DistinctBy(x => x.Image.Id);
                var completedtotalfinale = (from d in completedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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
                var completedtotalexportedfinalesheets = (from d in completedtotalexported select d).ToList().DistinctBy(x => x.Image.Id);
                // documents
                var completedtotalexportedfinale = (from d in completedtotalexported select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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
                var rescancompletedtotalfinalesheets = (from d in rescancompletedtotal select d).ToList().DistinctBy(x => x.Image.Id);
                var rescancompletedtotalfinale = (from d in rescancompletedtotal select d).ToList().DistinctBy(x => x.d.Id).OrderBy(x => x.d.PageOrder)
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



                return new Dashboard_DTO
                {

                    scannfini = $"{scantotalfinale} [ {Convert.ToInt32(scantotalfinalesheets.Count() / 2)} : sheets ] ",
                    scannedtotalfini = $"{scannedtotalfinale} [ {Convert.ToInt32(scannedtotalfinalesheets.Count() / 2)} : sheets ] ",
                    rescantotalfini = $"{rescantotalfinale} [ {Convert.ToInt32(rescantotalfinalesheets.Count() / 2)} : sheets ] ",
                    rescancompletedtotalfini = $"{rescancompletedtotalfinale} [ {Convert.ToInt32(rescancompletedtotalfinalesheets.Count() / 2)} : sheets ] ",
                    qualitytotalfini = $"{qualitytotalfinale} [ {Convert.ToInt32(qualitytotalfinalesheets.Count() / 2)} : sheets ] ",
                    completedtotalfini = $"{completedtotalfinale} [ {Convert.ToInt32(completedtotalfinalesheets.Count() / 2)} : sheets ] ",
                    completedtotalexportedfinale = $"{completedtotalexportedfinale} [ {Convert.ToInt32(completedtotalexportedfinalesheets.Count() / 2)} : sheets ] ",
                    UserCounts = finalList
                };


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        ///  get first documents if we click on batch detail to shoz it directly
        ///     if step == 12 ( batch to rescan ) we get the docs corresponding on btahc who has a status 2 
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        public async Task<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO> getFirstDocument(BatchwithStepRequest batchwithStep)
        {
            try
            {
                var data = batchwithStep.batchId;

                var statusall = new List<long?> { 4 };
                var stepall = new List<long?> { batchwithStep.step };
                var doctype = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;
                if (batchwithStep.step == 12)
                {
                    statusall = new List<long?> { 5, 6 };
                    stepall = new List<long?> { 2, 3, 5, 8 };
                }
                if (batchwithStep.step == 11)
                {
                    statusall = new List<long?> { 1, 2, 4, 5, 6 };
                    stepall = new List<long?> { 2, 3, 5, 8 };
                }
                if (batchwithStep.step == 14)
                {
                    statusall = new List<long?> { 1 };
                    stepall = new List<long?> { 8 };
                }

                var query = from f in exorabilisContext.Files
                            join d in exorabilisContext.Document on f.Id equals d.FileId
                            join b in exorabilisContext.Batch on f.BatchId equals b.Id
                            join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                            join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id into statusfinal
                            from status in statusfinal.DefaultIfEmpty()
                            join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id into stepFinal
                            join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id into typefinal
                            from type in typefinal.DefaultIfEmpty()

                            where stepall.Contains(d.DocumentStepId) &&
                         statusall.Contains(d.DocumentStatusId) &&
                            f.BatchId == Convert.ToInt64(batchwithStep.batchId) &&
                            (d.DocumentTypeId == Convert.ToInt64(doctype) || d.DocumentTypeId == 7 ) && ir.Status == 1
                            select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                            {
                                IdEncrypt = d.Id.ToString(),
                                FileId = f.Id,
                                BatchNumber = b.ReferenceNumber,
                                DocumentNumber = d.ReferenceNumber,
                                DocumentTypeName = type.Name,
                                DocumentStatusName = status.Name,
                                CroppedOn = d.CroppedOn,
                                PageOrder = d.PageOrder,
                                LastUpdatedOn = d.LastUpdatedOn,
                                IndexedOn = d.IndexedOn,
                                IndexedBy = d.IndexedBy,
                                CroppedBy = d.CroppedBy,
                                ReasonOther = d.ReasonOther,
                                Pcname = d.Pcname,

                                CreatedBy = d.CreatedBy,
                                LastStep = d.LastStep,
                                ModifiedBy = d.ModifiedBy,
                                ModifiedOn = d.ModifiedOn,
                                QualityBy = d.QualityBy,
                                QualityOn = d.QualityOn,

                                RescanBy = d.RescanBy,
                                RescanOn = d.RescanOn,
                                RejectedBy = d.RejectedBy,
                                RejectedOn = d.RejectedOn,

                                SanityOn = d.SanityOn,
                                SanityBy = d.SanityBy,
                                FinalQualityOn = d.FinalQualityOn,
                                FinalQualityBy = d.FinalQualityBy,
                                ReviewedOn = d.ReviewedOn,
                                ReviewedBy = d.ReviewedBy,
                                CreatedOn = d.CreatedOn,
                                FileTypeId = b.File.FileTypeId
                                //IndexModels = allindex

                            };


                var results = await query.Distinct().OrderBy(i => i.PageOrder).FirstOrDefaultAsync();




                return results;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList(string docId)
        {
            try
            {

                if (docId == null) return null;

                var batch = (from i in exorabilisContext.Document
                             join batches in exorabilisContext.Batch on i.BatchId equals batches.Id
                             where i.Id == Convert.ToInt64(docId)
                             select batches
                             ).FirstOrDefault();


                var docid = docId;
                var list = await (from pj in exorabilisContext.Project
                                  join pjtv in exorabilisContext.Project_folio on pj.Id equals pjtv.Project_id
                                  join pjt in exorabilisContext.Document_type on pjtv.Document_type_id equals pjt.Id
                                  join file in exorabilisContext.Files on pjtv.Id equals file.Project_folio_id into pfolio
                                  from file in pfolio.DefaultIfEmpty()
                                  where pj.Reference == batch.FileNumber
                                  select new DocumentIndex_DTO
                                  {
                                      Project_type_name = pjt.Name,
                                      Project_reference = pj.Reference,
                                      NbrOfPages = pjtv.NbrOfPages,
                                      Folios = pjtv.Folios,
                                      FileValueId = file.Id,
                                      Project_type_value_id = pjtv.Id
                                  }).ToListAsync();

                return list;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList()
        {
            try
            {
                var list = await (from i in exorabilisContext.Index
                                  join idt in exorabilisContext.IndexDataType on i.DataTypeId equals idt.Id
                                  where i.Status == 1
                                  orderby i.Order
                                  select new DocumentIndex_DTO
                                  {

                                  }).ToListAsync();

                return list;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentImage_DTO> getAllImageByDocId(string docId)
        {
            try
            {
                if (docId == null) return new DocumentImage_DTO();

                var docid = docId;
                var imageextension = interfaceBatchDataProvider.GetValueParam("imageExtension").Value;

                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                var doctype = Convert.ToInt64(doctypeid);

                var result = from img in exorabilisContext.Image
                             join docs in exorabilisContext.Document on img.DocumentId equals docs.Id
                             join batch in exorabilisContext.Batch on docs.BatchId equals batch.Id
                             join files in exorabilisContext.Files on docs.FileId equals files.Id
                             join filetype in exorabilisContext.FileType on files.FileTypeId equals filetype.Id
                             where docs.Id == Convert.ToInt64(docid) && img.Status == 1
                             select new
                             {
                                 DocumentNumber = docs.ReferenceNumber,
                                 DocumentStep = docs.DocumentStepId,
                                 DocumentStatus = docs.DocumentStatusId,
                                 DocumentId = img.DocumentId,
                                 Image = img,
                                 qualitedBy = docs.QualityBy,
                                 otherreason = docs.ReasonOther,
                                 rejectid = docs.RejectionCodeId,
                                 filetype = filetype,
                                 fileId = docs.FileId,
                                 filenumber = batch.FileNumber


                             } into temp
                             group temp by temp.DocumentId into grouped
                             select new DocumentImage_DTO
                             {
                                 DocsCount = (from img in exorabilisContext.Image
                                              join docs in exorabilisContext.Document on img.DocumentId equals docs.Id
                                              where (docs.DocumentTypeId == doctype || docs.DocumentTypeId == 7 ) && docs.FileId == Convert.ToInt64(grouped.FirstOrDefault().fileId) && img.Status == 1
                                              select docs
                                             ).Count(),
                                 IdVerso = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.Id.ToString(),
                                 IdRecto = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.Id.ToString(),
                                 IdEncrypt = docId,
                                 FileId = grouped.FirstOrDefault().fileId,
                                 FileNumber = grouped.FirstOrDefault().filenumber,
                                 isQualited = (string.IsNullOrEmpty(grouped.FirstOrDefault().qualitedBy) ? false : true),
                                 rejectioncodeid = grouped.FirstOrDefault().rejectid,
                                 otherreason = grouped.FirstOrDefault().otherreason,
                                 DocumentStep = grouped.FirstOrDefault().DocumentStep,
                                 DocumentStatus = grouped.FirstOrDefault().DocumentStatus,
                                 FileTypeId = grouped.FirstOrDefault().filetype.Id,
                                 DocumentNumber = grouped.FirstOrDefault().DocumentNumber,
                                 ImageContentStr = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : $"data:image/{imageextension};base64, " + grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.ImageToBase64String,
                                 PathRecto = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.Path,
                                 ImageVersoContentStr = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : $"data:image/{imageextension};base64, " + grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.ImageToBase64String,
                                 PathVerso = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.Path,

                             };
                var resultat = await result.FirstOrDefaultAsync();



                return resultat;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<string> SaveImageCropped(IConfiguration _configuration, IWebHostEnvironment webhostEnvironment, string docId, string BatchId, string documentnumber, string Image, bool isRecto, string UserId, string UserName)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();

            try
            {

                var docid = docId;

                Batch batch = await (
                    from b in exorabilisContext.Batch
                    join file in exorabilisContext.Files on b.Id equals file.BatchId
                    join docs in exorabilisContext.Document on file.Id equals docs.FileId
                    where docs.Id == Convert.ToInt64(docid)
                    select b
                              ).FirstOrDefaultAsync();


                if (string.IsNullOrEmpty(Image) || Image == "{}" || batch == null)
                    return Image;


                var imageextension = interfaceBatchDataProvider.GetValueParam("imageExtension").Value;

                Byte[] bitmapData = Convert.FromBase64String(Image.Replace("data:image/" + imageextension + ";base64,", ""));


                var physicalPath = interfaceBatchDataProvider.GetValueParam("ImagePathWeb").Value;
                var isPath = interfaceBatchDataProvider.getChrono().Result.DatabaseSaveImage;

                string webRootPath = webhostEnvironment.WebRootPath;
                string contentRootPath = webhostEnvironment.ContentRootPath;
                var newImagePath = "";
                var exportedImagePath = string.Empty;
                var filePath = "";

                var RecVer = isRecto ? "_R" : "_V";

                newImagePath = physicalPath + batch?.ReferenceNumber + "/" + documentnumber + RecVer + "." + imageextension;
                filePath = webRootPath + newImagePath;

                string directoryPath = Path.GetDirectoryName(filePath);

                if (isPath == 0)
                {
                    // Crée le répertoire s'il n'existe pas déjà
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (MagickImage image = new MagickImage(bitmapData))
                        image.Write(filePath);

                    Image = newImagePath;
                }
                else
                {
                    Image images = null;

                    RecVer = isRecto ? "RECTO" : "VERSO";

                    images = await exorabilisContext?.Image?.FirstOrDefaultAsync(x => x.DocumentId == Convert.ToInt64(docid) && x.Side == RecVer);

                    images.ImageToBase64String = Image.Replace("data:image/" + imageextension + ";base64,", "");

                }

                Document doc = await this.exorabilisContext.Document.FirstOrDefaultAsync(x => x.Id == Convert.ToInt64(docid));
                if (doc != null)
                {
                    doc.CroppedBy = UserName;
                    doc.CroppedOn = DateTime.Now;
                }

                await exorabilisContext?.SaveChangesAsync();


                await transaction.CommitAsync();

                return Image;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task<DocumentImage_DTO> getImagesOfNextDocumentsIFMultiByDocIdNext(string docId, string referenceNumber, long fileTypeId, string nextorprev, long step)
        {
            try
            {
                var docid = docId;
                var imageextension = interfaceBatchDataProvider.GetValueParam("imageExtension").Value;

                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                var doctype = Convert.ToInt64(doctypeid);

                if (fileTypeId == 2)
                {
                    if (step.ToString().Contains("12"))
                    {
                        List<long> listofstep = new List<long> { 2, 3, 5, 8 };

                    }
                    else
                    {

                    }

                    string[] parts = referenceNumber.Split('.');
                    var actual = this.exorabilisContext.Document.FirstOrDefault(x => x.Id == Convert.ToInt64(docid));
                    // Récupérer le prochain document dans la base de données basé sur l'ID
                    var nextDocOrPrev = new Document();
                    if (nextorprev == "next")
                    {
                        nextDocOrPrev = (from x in exorabilisContext.Document
                                         join Image in exorabilisContext.Image on x.Id equals Image.DocumentId
                                         where Image.Status == 1 && x.PageOrder > actual.PageOrder && ( x.DocumentTypeId == doctype || x.DocumentTypeId == 7 ) && x.FileId == actual.FileId && ((x.DocumentStatusId != 5 && x.DocumentStatusId != 6 && step != 12) || (x.DocumentStatusId == 5 || x.DocumentStatusId == 6 && step == 12))
                                         select x)
                                        .OrderBy(x => x.PageOrder) // Assurer que l'on récupère les documents dans l'ordre des IDs
                                        .FirstOrDefault();

                    }
                    else
                    {
                        nextDocOrPrev = (from x in exorabilisContext.Document
                                         join Image in exorabilisContext.Image on x.Id equals Image.DocumentId
                                         where Image.Status == 1 && x.PageOrder < actual.PageOrder && (x.DocumentTypeId == doctype || x.DocumentTypeId == 7) && x.FileId == actual.FileId && (( x.DocumentStatusId != 5 && x.DocumentStatusId != 6 && step != 12) || (x.DocumentStatusId == 5 || x.DocumentStatusId == 6 && step == 12))
                                         select x)
                            .OrderBy(x => x.PageOrder) // Assurer que l'on récupère les documents dans l'ordre des IDs
                            .LastOrDefault();


                    }

                    if (nextDocOrPrev != null)
                    {
                        // Le prochain document trouvé
                        docid = nextDocOrPrev.Id.ToString();
                    }
                    else
                    {
                        // Si aucun document suivant n'est trouvé
                        //docid = "Aucun document suivant trouvé";
                    }

                }

                var result = from img in exorabilisContext.Image
                             join docs in exorabilisContext.Document on img.DocumentId equals docs.Id

                             join files in exorabilisContext.Files on docs.FileId equals files.Id
                             join filetype in exorabilisContext.FileType on files.FileTypeId equals filetype.Id
                             where docs.Id == Convert.ToInt64(docid)
                             select new
                             {

                                 DocumentNumber = docs.ReferenceNumber,
                                 DocumentStep = docs.DocumentStepId,
                                 DocumentStatus = docs.DocumentStatusId,
                                 DocumentId = img.DocumentId,
                                 Image = img,
                                 filetype = filetype,
                                 fileId = docs.FileId

                             } into temp
                             group temp by temp.DocumentId into grouped
                             select new DocumentImage_DTO
                             {
                                 IdVerso = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.Id.ToString(),
                                 IdRecto = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.Id.ToString(),
                                 IdEncrypt = grouped.FirstOrDefault().DocumentId.ToString(),
                                 DocumentStep = grouped.FirstOrDefault().DocumentStep,
                                 FileId = grouped.FirstOrDefault().fileId,

                                 DocumentStatus = grouped.FirstOrDefault().DocumentStatus,
                                 FileTypeId = grouped.FirstOrDefault().filetype.Id,
                                 DocumentNumber = grouped.FirstOrDefault().DocumentNumber,
                                 ImageContentStr = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : $"data:image/{imageextension};base64, " + grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.ImageToBase64String,
                                 PathRecto = grouped.FirstOrDefault(x => x.Image.Side == "RECTO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "RECTO").Image.Path,
                                 ImageVersoContentStr = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : $"data:image/{imageextension};base64, " + grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.ImageToBase64String,
                                 PathVerso = grouped.FirstOrDefault(x => x.Image.Side == "VERSO" && x.Image.Status == 1) == null ? string.Empty : grouped.FirstOrDefault(x => x.Image.Side == "VERSO").Image.Path,

                             };
                var resultat = await result.FirstOrDefaultAsync(); //jiji

                return resultat;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public long GetNextStepId(long currentStepId)
        {
            var nextStep = exorabilisContext.WorkFlow.AsNoTracking().Where(x => x.FromStepId == currentStepId).FirstOrDefault();
            if (nextStep != null)
                return nextStep.ToStepId;

            return 0;
        }

        public Document SetActionDoneBy(Document document, string userName)
        {
            if (document != null)
            {
                switch (document.DocumentStepId)
                {
                    case 2: //TODO WIPO
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



        public Document SetActionAfterReview(Document document, long? step, string userName)
        {
            if (document != null)
            {
                switch (step)
                {
                    case 2:
                        document.QualityBy = userName;
                        document.QualityOn = DateTime.Now;
                        break;
                    case 3:
                        document.QualityBy = userName;
                        document.QualityOn = DateTime.Now;

                        break;
                    case 5:
                        document.IndexedBy = userName;
                        document.IndexedOn = DateTime.Now;
                        break;

                    case 8:
                        document.ReviewedBy = userName;
                        document.ReviewedOn = DateTime.Now;
                        break;
                }
            }

            return document;
        }

        public async Task<SaveIndexResponse> SaveIndexation(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev, string otherreason, string step)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();
            try
            {

                var docid = Convert.ToInt64(docId);
                var indexedDocuments = 0;
                var indexedDocuments2 = 0;
                var batchid = "";
                reason = (reason == 0) ? null : reason;

                var responseofall = new SaveIndexResponse();

                IEnumerable<DocumentIndex> docIndexes;

                var document = exorabilisContext.Document.Where(x => x.Id == docid).FirstOrDefault();
                var fileOfDocument = exorabilisContext.Files.Where(x => x.Id == document.FileId).FirstOrDefault();

                if (document != null)
                {

                    var oldStep = document.DocumentStepId;

                    var batch = (
                        from b in exorabilisContext.Batch
                        join file in exorabilisContext.Files on b.Id equals file.BatchId
                        join doc in exorabilisContext.Document on file.Id equals doc.FileId
                        where doc.Id == docid
                        select b).FirstOrDefault();

                    var project = (from p in exorabilisContext.Project where p.Reference == batch.FileNumber select p).FirstOrDefault();

                    var project_folio = this.exorabilisContext.Project_folio.Where(x => x.Id == Project_folio_id).FirstOrDefault();

                    var nes = (int)this.GetNextStepId(batch.DocumentStepId.Value);


                    if (type == 3)
                    {
                        if (fileTypeId == 2)
                        {
                            var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                            if (multiDocuments != null)
                            {
                                foreach (var multiDocument in multiDocuments)
                                {
                                    multiDocument.DocumentStatusId = 5;
                                    multiDocument.ReasonOther = otherreason;
                                    multiDocument.RejectionCodeId = reason;
                                    multiDocument.RescanBy = UserName;
                                    multiDocument.RescanOn = DateTime.Now;
                                    multiDocument.ReviewedBy = UserName;
                                    multiDocument.ReviewedOn = DateTime.Now;

                                    document.LastStep = "Rescan";

                                    //batch.NumberOfDocument -= 1;
                                    batch.RescanBy = UserName;
                                    batch.RescanOn = DateTime.Now;
                                }
                            }
                        }
                        else
                        {

                            document.DocumentStatusId = 5;
                            document.ReasonOther = otherreason;
                            document.RejectionCodeId = reason;
                            document.ReviewedBy = UserName;
                            document.ReviewedOn = DateTime.Now;
                            document.RescanBy = UserName;
                            document.RescanOn = DateTime.Now;
                            document.LastStep = "Rescan";

                            //batch.NumberOfDocument -= 1;
                            batch.RescanBy = UserName;
                            batch.RescanOn = DateTime.Now;
                            batch.LastStep = "Rescan";

                        }

                    }
                    else
                    {



                        batchid = batch.Id.ToString();

                        var nextStep = 0;

                        this.interfaceBatchDataProvider.SetActionDoneByBatch(batch, UserName);


                        nextStep = type == 1 ? (int)nes : 8;

                        var multiDocs = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                        if (multiDocs != null)
                        {

                            if (Project_folio_id != null)
                            {

                                var temp = exorabilisContext.Files.FirstOrDefault(x => x.BatchId == batch.Id && x.Id != document.FileId && x.Project_folio_id == Project_folio_id);


                                if (temp != null)
                                {

                                    responseofall.isReturn = 1;
                                    responseofall.indexdocument = indexedDocuments;
                                    responseofall.documentnumberEdited = $"{document.FileId}";
                                    responseofall.documentnumberOccurrence = $"{temp.Id}";
                                    responseofall.champunique = "Folio";

                                    return responseofall;

                                }
                                else
                                {
                                    fileOfDocument.Project_folio_id = Project_folio_id;
                                    project_folio.FileId = document.FileId;
                                    responseofall.isReturn = 0;
                                }

                            }

                        }

                        var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                        if (multiDocuments != null)
                        {
                            foreach (var multiDocument in multiDocuments)
                            {
                                var tempmultiDocument = SetActionDoneBy(multiDocument, UserName);

                                if (tempmultiDocument.DocumentStatusId == 2)
                                {
                                    tempmultiDocument.DocumentStepId = batch.DocumentStepId;

                                    tempmultiDocument = SetActionAfterReview(tempmultiDocument, batch.DocumentStepId, UserName);
                                }
                                else
                                {
                                    tempmultiDocument.DocumentStepId = nextStep;
                                }

                                if (type == 1)
                                {
                                    tempmultiDocument.RejectionCodeId = null;

                                    if ((tempmultiDocument.DocumentStatusId == 4) && oldStep == 8)
                                    {
                                        tempmultiDocument.DocumentStatusId = 1;
                                    }
                                    else if (batch.DocumentStepId == 8)
                                    {

                                        tempmultiDocument.DocumentStatusId = 1;
                                    }
                                    else
                                    {

                                        tempmultiDocument.DocumentStatusId = 1; // normale c'est 4 : mais pour EUIPO : on saute directement vers prete a etre exporter 1

                                    }

                                }
                                else if (type == 2)
                                {
                                    tempmultiDocument.DocumentStatusId = 2;
                                    tempmultiDocument.RejectionCodeId = reason;
                                    tempmultiDocument.ReasonOther = otherreason;

                                    tempmultiDocument.RejectedBy = UserName;
                                    tempmultiDocument.RejectedOn = DateTime.Now;

                                }

                                var multiDocumentHistory = new DocumentHistory();
                                multiDocumentHistory.BatchId = batch.Id;
                                multiDocumentHistory.DocumentId = tempmultiDocument.Id;
                                multiDocumentHistory.DocumentStepId = tempmultiDocument.DocumentStepId;
                                multiDocumentHistory.DocumentStatusId = tempmultiDocument.DocumentStatusId;
                                multiDocumentHistory.RejectionCodeId = tempmultiDocument.RejectionCodeId;
                                multiDocumentHistory.UserId = UserId;
                                multiDocumentHistory.UserName = UserName;
                                multiDocumentHistory.RejectionCodeId = reason;

                                exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                            }
                        }

                    }

                    // verif si il y a encore celle qui ne sont pas 
                    if (fileTypeId == 2)
                    {


                        var fileIds2 = (from b in exorabilisContext.Document
                                        join f in exorabilisContext.Files on b.FileId equals f.Id
                                        where f.BatchId == batch.Id && (b.DocumentStepId == 2) && (b.DocumentStatusId == 4) && b.DocumentTypeId == document.DocumentTypeId
                                        select new { b.FileId }).Distinct();

                        indexedDocuments2 = fileIds2.Count();


                    }
                    else
                    {

                        indexedDocuments2 = exorabilisContext.Document.Count(x => x.FileId == document.FileId && (x.DocumentStepId == 2) && (x.DocumentStatusId == 4) && x.DocumentTypeId == document.DocumentTypeId);

                    }

                    exorabilisContext.SaveChanges();

                    var verififhasdocument = (from b in exorabilisContext.Document
                                              where b.BatchId == document.BatchId &&
                                               b.DocumentStepId == 2 && b.DocumentStatusId == 4
                                              //stat.Contains(b.DocumentStatusId) && steps.Contains(b.DocumentStepId) 
                                              && b.DocumentTypeId == document.DocumentTypeId
                                              select b).Distinct();

                    if (verififhasdocument.Count() == 0)
                    {
                        if (batch.DocumentStepId == 8) // si batch est deja 8
                        {
                            this.interfaceBatchDataProvider.SetActionDoneByBatch(batch, UserName);
                            batch.DocumentStatusId = 1;
                            batch.LockedBy = string.Empty;
                            batch.IsLocked = null;
                            interfaceBatchDataProvider.createOrUpdateBatchHistory(batch, UserId, null);
                        }
                        else if (batch.DocumentStepId != 8 && oldStep != 8)
                        {
                            this.interfaceBatchDataProvider.SetActionDoneByBatch(batch, UserName);
                            batch.DocumentStepId = nes;
                            batch.DocumentStatusId = 1; // normale c'est 4 : mais pour EUIPO : on saute directement vers prete a etre exporter 1
                            batch.LockedBy = string.Empty;
                            batch.IsLocked = null;
                            interfaceBatchDataProvider.createOrUpdateBatchHistory(batch, UserId, null);
                        }
                        exorabilisContext.SaveChanges();

                    }


                }



                transaction.Commit();

                var response = new SaveIndexResponse
                {
                    indexdocument = indexedDocuments2,
                    isReturn = 0
                };

                return response;


            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }

        private SaveIndexResponse checkunique(int indexedDocuments, Document? document, Document? multiDocument, ValueIndexRequest item, long? filetype)
        {
            var verifunique = exorabilisContext.Index.FirstOrDefault(x => x.Id == item.IndexId && x.IsUnique == 1);

            var responses = new SaveIndexResponse();

            if (verifunique != null)
            {

                if (filetype == 2)
                {
                    var verifdoc = (from ddocindex in exorabilisContext.DocumentIndex
                                    join doc in exorabilisContext.Document on ddocindex.DocumentId equals doc.Id
                                    where doc.DocumentStatusId != 5 && doc.DocumentStatusId != 6 && ddocindex.Value == item.Value && doc.FileId != multiDocument.FileId
                                    select doc).FirstOrDefault();

                    if (verifdoc != null)
                    {

                        responses.isReturn = 1;
                        responses.indexdocument = indexedDocuments;
                        responses.documentnumberEdited = document.ReferenceNumber;
                        responses.documentnumberOccurrence = verifdoc.ReferenceNumber;
                        responses.champunique = verifunique.Name;


                    }
                    else
                    {
                        responses.isReturn = 0;

                    }

                }
                else
                {
                    var verifdoc = (from ddocindex in exorabilisContext.DocumentIndex
                                    join doc in exorabilisContext.Document on ddocindex.DocumentId equals doc.Id
                                    where doc.DocumentStatusId != 5 && doc.DocumentStatusId != 6 && ddocindex.Value == item.Value && doc.Id != multiDocument.Id
                                    select doc).FirstOrDefault();

                    if (verifdoc != null)
                    {

                        responses.isReturn = 1;
                        responses.indexdocument = indexedDocuments;
                        responses.documentnumberEdited = document.ReferenceNumber;
                        responses.documentnumberOccurrence = verifdoc.ReferenceNumber;
                        responses.champunique = verifunique.Name;

                    }
                    else
                    {
                        responses.isReturn = 0;

                    }
                }


            }
            else
            {
                responses.isReturn = 0;
            }

            return responses;
        }


        public async Task<bool> SaveAllDocumentsReview(DocumentSearchRequest request)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();


            try
            {

                var reqstep = Convert.ToInt64(request.step.ToString());

                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                var docstatus = new List<long> { 4 };
                var docstep = new List<long> { 8 };
                var doctype = new List<long> { Convert.ToInt64(doctypeid) };

                var data = (from d in exorabilisContext.Document
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

                    data = from d in data
                           where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                           (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) >= request.firstdate) ||
                           (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) >= request.firstdate)
                           select d;

                }

                if (request.lastdate != null)
                {
                    data = from d in data
                           where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                            (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) <= request.lastdate) ||
                            (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) <= request.lastdate)
                           select d;
                }

                if (request.batchnumber != null)
                {
                    data = from d in data
                           where d.b.ReferenceNumber.Contains(request.batchnumber)
                           select d;
                }

                if (request.documentnumber != null)
                {
                    data = from d in data
                           where d.d.ReferenceNumber.Contains(request.documentnumber)
                           select d;
                }

                var result = (from d in data
                              select new
                              {
                                  Id = d.d.Id,
                                  FileId = d.f.Id,
                                  FileTypeId = d.b.File.FileTypeId,

                              }).ToList().DistinctBy(x => x.Id);




                var results = result
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

                var listIn = results.Select(x => x.Id).ToList();
                var listInMulti = results.Select(x => x.FileId).DistinctBy(s => s).ToList();

                List<long?> listId = new List<long?> { 2, 4 };

                foreach (var item in results)
                {
                    var docid = item.Id;
                    var batchid = "";

                    var document = exorabilisContext.Document.Where(x => x.Id == docid).FirstOrDefault();

                    if (document != null)
                    {


                        var batch = (
                       from b in exorabilisContext.Batch
                       join file in exorabilisContext.Files on b.Id equals file.BatchId
                       join doc in exorabilisContext.Document on file.Id equals doc.FileId
                       where doc.Id == docid
                       select b).FirstOrDefault();

                        var oldStep = document.DocumentStepId;


                        batchid = batch.Id.ToString();

                        var nextStep = 0;

                        var nes = (int)this.GetNextStepId(batch.DocumentStepId.Value);

                        nextStep = (int)nes;


                        if (item.FileTypeId == 2)
                        {

                            var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                            if (multiDocuments != null)
                            {
                                foreach (var multiDocument in multiDocuments)
                                {
                                    var tempmultiDocument = SetActionDoneBy(multiDocument, request.UserName);


                                    tempmultiDocument.DocumentStepId = nextStep;


                                    tempmultiDocument.RejectionCodeId = null;

                                    tempmultiDocument.DocumentStatusId = 1;

                                    var multiDocumentHistory = new DocumentHistory();
                                    multiDocumentHistory.BatchId = batch.Id;
                                    multiDocumentHistory.DocumentId = tempmultiDocument.Id;
                                    multiDocumentHistory.DocumentStepId = nextStep;
                                    multiDocumentHistory.DocumentStatusId = tempmultiDocument.DocumentStatusId;
                                    multiDocumentHistory.RejectionCodeId = tempmultiDocument.RejectionCodeId;
                                    multiDocumentHistory.UserId = request.userId;
                                    multiDocumentHistory.UserName = request.UserName;

                                    exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                                }
                            }

                        }
                        else
                        {


                            document = SetActionDoneBy(document, request.UserName);

                            document.DocumentStepId = nextStep;

                            document.DocumentStatusId = 1;

                            var documentHistory = new DocumentHistory();
                            documentHistory.BatchId = batch.Id;
                            documentHistory.DocumentId = document.Id;
                            documentHistory.DocumentStepId = nextStep;
                            documentHistory.DocumentStatusId = document.DocumentStatusId;
                            documentHistory.RejectionCodeId = document.RejectionCodeId;
                            documentHistory.UserId = request.userId;
                            documentHistory.UserName = request.UserName;

                            exorabilisContext.DocumentHistory.Add(documentHistory);


                        }

                        if (batch.DocumentStepId == 8)
                        {
                            List<long?> stat = new List<long?> { 2, 4 };
                            int indexedDocuments1 = 0;
                            if (item.FileTypeId == 2)
                            {

                                var fileIds = (from b in exorabilisContext.Document
                                               join f in exorabilisContext.Files on b.FileId equals f.Id
                                               where (b.DocumentStatusId != 5 && b.DocumentStatusId != 6) && f.BatchId == batch.Id && !listInMulti.Contains(b.FileId.Value) && stat.Contains(b.DocumentStatusId) && b.DocumentTypeId == document.DocumentTypeId
                                               select new { b.FileId }).Distinct();

                                indexedDocuments1 = fileIds.Count();


                            }
                            else
                            {

                                indexedDocuments1 = exorabilisContext.Document.Count(x => x.FileId == document.FileId && (x.DocumentStatusId != 5 && x.DocumentStatusId != 6) && !listIn.Contains(x.Id) && stat.Contains(x.DocumentStatusId) && x.DocumentTypeId == document.DocumentTypeId);

                            }

                            if (indexedDocuments1 == 0)
                            {

                                batch.DocumentStatusId = 1;
                                batch.LockedBy = string.Empty;
                                batch.IsLocked = null;

                                this.interfaceBatchDataProvider.SetActionDoneByBatch(batch, request.UserName);

                                interfaceBatchDataProvider.createOrUpdateBatchHistory(batch, request.userId, null);

                            }


                        }


                    }



                }

                exorabilisContext.SaveChanges();

                transaction.Commit();


                return true;



            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }


        }

        public async Task<bool> RescanAllDocumentsReview(DocumentSearchRequest request)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();


            try
            {

                var reqstep = Convert.ToInt64(request.step.ToString());

                var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                var docstatus = new List<long> { 2 };
                var docstep = new List<long> { 8 };
                var doctype = new List<long> { Convert.ToInt64(doctypeid) };

                var data = (from d in exorabilisContext.Document
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

                    data = from d in data
                           where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) >= request.firstdate) ||
                           (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) >= request.firstdate) ||
                           (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) >= request.firstdate)
                           select d;

                }

                if (request.lastdate != null)
                {
                    data = from d in data
                           where (d.d.CreatedOn.HasValue && DateOnly.FromDateTime(d.d.CreatedOn.Value) <= request.lastdate) ||
                            (d.d.SanityOn.HasValue && DateOnly.FromDateTime(d.d.SanityOn.Value) <= request.lastdate) ||
                            (d.d.RejectedOn.HasValue && DateOnly.FromDateTime(d.d.RejectedOn.Value) <= request.lastdate)
                           select d;
                }

                if (request.batchnumber != null)
                {
                    data = from d in data
                           where d.b.ReferenceNumber.Contains(request.batchnumber)
                           select d;
                }

                if (request.documentnumber != null)
                {
                    data = from d in data
                           where d.d.ReferenceNumber.Contains(request.documentnumber)
                           select d;
                }

                var result = (from d in data
                              select new
                              {
                                  FileId = d.f.Id,
                                  Id = d.d.Id,
                                  FileTypeId = d.b.File.FileTypeId

                              }).ToList().DistinctBy(x => x.Id);




                var results = result
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

                var listIn = results.Select(x => x.Id).ToList();
                var listInMulti = results.Select(x => x.FileId).DistinctBy(s => s).ToList();

                foreach (var item in results)
                {
                    var docid = item.Id;
                    var batchid = "";

                    var document = exorabilisContext.Document.Where(x => x.Id == docid).FirstOrDefault();

                    if (document != null)
                    {


                        var batch = (
                       from b in exorabilisContext.Batch
                       join file in exorabilisContext.Files on b.Id equals file.BatchId
                       join doc in exorabilisContext.Document on file.Id equals doc.FileId
                       where doc.Id == docid
                       select b).FirstOrDefault();

                        var oldStep = document.DocumentStepId;


                        batchid = batch.Id.ToString();

                        var nextStep = 0;

                        var nes = (int)this.GetNextStepId(batch.DocumentStepId.Value);

                        nextStep = (int)nes;



                        if (item.FileTypeId == 2)
                        {
                            var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                            if (multiDocuments != null)
                            {
                                foreach (var multiDocument in multiDocuments)
                                {
                                    multiDocument.DocumentStatusId = 5;
                                    multiDocument.ReasonOther = request.otherreason;
                                    multiDocument.RejectionCodeId = request.reason;

                                    multiDocument.RescanBy = request.UserName;
                                    multiDocument.RescanOn = DateTime.Now;
                                    multiDocument.ReviewedBy = request.UserName;
                                    multiDocument.ReviewedOn = DateTime.Now;

                                    document.LastStep = "Rescan";
                                    batch.NumberOfDocument -= 1;


                                    var multiDocumentHistory = new DocumentHistory();
                                    multiDocumentHistory.BatchId = batch.Id;
                                    multiDocumentHistory.DocumentId = multiDocument.Id;
                                    multiDocumentHistory.DocumentStepId = multiDocument.DocumentStepId;
                                    multiDocumentHistory.DocumentStatusId = multiDocument.DocumentStatusId;
                                    multiDocumentHistory.RejectionCodeId = multiDocument.RejectionCodeId;
                                    multiDocumentHistory.UserId = request.userId;
                                    multiDocumentHistory.UserName = request.UserName;

                                    exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                                }
                            }
                        }
                        else
                        {

                            document.DocumentStatusId = 5;
                            document.ReasonOther = request.otherreason;
                            document.RejectionCodeId = request.reason;

                            document.RescanBy = request.UserName;
                            document.RescanOn = DateTime.Now;
                            document.ReviewedBy = request.UserName;
                            document.ReviewedOn = DateTime.Now;

                            document.LastStep = "Rescan";

                            batch.NumberOfDocument -= 1;


                            var documentHistory = new DocumentHistory();
                            documentHistory.BatchId = batch.Id;
                            documentHistory.DocumentId = document.Id;
                            documentHistory.DocumentStepId = document.DocumentStepId;
                            documentHistory.DocumentStatusId = document.DocumentStatusId;
                            documentHistory.RejectionCodeId = document.RejectionCodeId;
                            documentHistory.UserId = request.userId;
                            documentHistory.UserName = request.UserName;

                            exorabilisContext.DocumentHistory.Add(documentHistory);

                        }

                        if (batch.DocumentStepId == 8)
                        {
                            List<long?> stat = new List<long?> { 2, 4 };
                            int indexedDocuments1 = 0;
                            if (item.FileTypeId == 2)
                            {

                                var fileIds = (from b in exorabilisContext.Document
                                               join f in exorabilisContext.Files on b.FileId equals f.Id
                                               where (b.DocumentStatusId != 5 && b.DocumentStatusId != 6) && f.BatchId == batch.Id && !listInMulti.Contains(b.FileId.Value) && stat.Contains(b.DocumentStatusId) && b.DocumentTypeId == document.DocumentTypeId
                                               select new { b.FileId }).Distinct();

                                indexedDocuments1 = fileIds.Count();


                            }
                            else
                            {

                                indexedDocuments1 = exorabilisContext.Document.Count(x => x.FileId == document.FileId && (x.DocumentStatusId != 5 && x.DocumentStatusId != 6) && !listIn.Contains(x.Id) && stat.Contains(x.DocumentStatusId) && x.DocumentTypeId == document.DocumentTypeId);

                            }

                            if (indexedDocuments1 == 0)
                            {

                                batch.DocumentStatusId = 1;
                                batch.LockedBy = string.Empty;
                                batch.IsLocked = null;
                                this.interfaceBatchDataProvider.SetActionDoneByBatch(batch, request.UserName);
                                interfaceBatchDataProvider.createOrUpdateBatchHistory(batch, request.userId, null);

                            }


                        }

                    }



                }

                exorabilisContext.SaveChanges();

                transaction.Commit();


                return true;



            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }


        }



        public async Task<SaveIndexResponse> SaveModification(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev)
        {
            using var transaction = exorabilisContext.Database.BeginTransaction();
            try
            {
                reason = (reason == 0) ? null : reason;

                var docid = Convert.ToInt64(docId);
                var batchid = "";
                var responseofall = new SaveIndexResponse();

                IEnumerable<DocumentIndex> docIndexes;

                var document = exorabilisContext.Document.Where(x => x.Id == docid).FirstOrDefault();
                var fileOfDocument = exorabilisContext.Files.Where(x => x.Id == document.FileId).FirstOrDefault();

                if (document != null)
                {

                    var oldStep = document.DocumentStepId;

                    var batch = (
                    from b in exorabilisContext.Batch
                    join file in exorabilisContext.Files on b.Id equals file.BatchId
                    join doc in exorabilisContext.Document on file.Id equals doc.FileId
                    where doc.Id == docid
                    select b).FirstOrDefault();

                    var project = (from p in exorabilisContext.Project where p.Reference == batch.FileNumber select p).FirstOrDefault();

                    var project_folio = this.exorabilisContext.Project_folio.Where(x => x.Id == Project_folio_id).FirstOrDefault();

                    batchid = batch.Id.ToString();

                    var nextStep = 0;


                    nextStep = type == 1 ? (int)this.GetNextStepId(batch.DocumentStepId.Value) : 8;


                    var multiDocs = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                    if (multiDocs != null)
                    {

                        if (Project_folio_id != null)
                        {

                            var temp = exorabilisContext.Files.FirstOrDefault(x => x.BatchId == batch.Id && x.Id != document.FileId && x.Project_folio_id == Project_folio_id);


                            if (temp != null)
                            {

                                responseofall.isReturn = 1;
                                responseofall.indexdocument = 0;
                                responseofall.documentnumberEdited = $"{document.FileId}";
                                responseofall.documentnumberOccurrence = $"{temp.Id}";
                                responseofall.champunique = "Folio";

                                return responseofall;

                            }
                            else
                            {
                                fileOfDocument.Project_folio_id = Project_folio_id;
                                project_folio.FileId = document.FileId;
                                responseofall.isReturn = 0;
                            }

                        }

                    }



                    if (fileTypeId == 2)
                    {

                        var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                        if (multiDocuments != null)
                        {
                            foreach (var tempmultiDocument in multiDocuments)
                            {
                                //var tempmultiDocument = SetActionDoneBy(multiDocument, UserName);

                                tempmultiDocument.ModifiedBy = UserName;
                                tempmultiDocument.ModifiedOn = DateTime.Now;
                                tempmultiDocument.LastStep = "Modified";

                                tempmultiDocument.DocumentStepId = oldStep;
                                if (type == 1)
                                {
                                    // tempmultiDocument.DocumentStatusId = 1;
                                }

                                else if (type == 2)
                                {
                                    tempmultiDocument.DocumentStatusId = 2;
                                    tempmultiDocument.RejectionCodeId = reason;
                                }

                                var multiDocumentHistory = new DocumentHistory();
                                multiDocumentHistory.BatchId = batch.Id;
                                multiDocumentHistory.DocumentId = tempmultiDocument.Id;
                                multiDocumentHistory.DocumentStepId = tempmultiDocument.DocumentStepId;
                                multiDocumentHistory.DocumentStatusId = tempmultiDocument.DocumentStatusId;
                                multiDocumentHistory.RejectionCodeId = tempmultiDocument.RejectionCodeId;
                                multiDocumentHistory.UserId = UserId;
                                multiDocumentHistory.UserName = UserName;
                                multiDocumentHistory.RejectionCodeId = reason;

                                exorabilisContext.DocumentHistory.Add(multiDocumentHistory);
                            }
                        }




                    }
                    else
                    {


                        //document = SetActionDoneBy(document, UserName);

                        document.ModifiedBy = UserName;
                        document.ModifiedOn = DateTime.Now;
                        document.LastStep = "Modified";
                        document.DocumentStepId = oldStep;
                        if (type == 1)
                        {
                            //document.DocumentStatusId = 1;
                        }

                        else if (type == 2)
                        {
                            document.DocumentStatusId = 2;
                            document.RejectionCodeId = reason;

                        }

                        var documentHistory = new DocumentHistory();
                        documentHistory.BatchId = batch.Id;
                        documentHistory.DocumentId = document.Id;
                        documentHistory.DocumentStepId = document.DocumentStepId;
                        documentHistory.DocumentStatusId = document.DocumentStatusId;
                        documentHistory.RejectionCodeId = document.RejectionCodeId;
                        documentHistory.UserId = UserId;
                        documentHistory.UserName = UserName;
                        documentHistory.RejectionCodeId = reason;

                        exorabilisContext.DocumentHistory.Add(documentHistory);


                    }


                }

                exorabilisContext.SaveChanges();

                transaction.Commit();


                responseofall.isReturn = 0;
                responseofall.indexdocument = 0;



                return responseofall;

            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }

        public async Task<SaveIndexResponse> Saveordereddocuments(long batchId, List<ValueOrderRequest> orderData)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();
            try
            {

                /* var doctype = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;

                 var query = from f in exorabilisContext.Files
                             join d in exorabilisContext.Document on f.Id equals d.FileId
                             join b in exorabilisContext.Batch on f.BatchId equals b.Id
                             join ir in exorabilisContext.Image on d.Id equals ir.DocumentId
                             join status in exorabilisContext.DocumentStatus on d.DocumentStatusId equals status.Id into statusfinal
                             from status in statusfinal.DefaultIfEmpty()
                             join step in exorabilisContext.DocumentStep on d.DocumentStatusId equals step.Id into stepFinal
                             from step in stepFinal.DefaultIfEmpty()
                             join type in exorabilisContext.DocumentType on d.DocumentTypeId equals type.Id into typefinal
                             from type in typefinal.DefaultIfEmpty()
                             where f.BatchId == Convert.ToInt64(batchId) &&
                                   d.DocumentTypeId == Convert.ToInt64(doctype)
                             select new DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO
                             {
                                 Id = d.Id,
                                 IdEncrypt = d.Id.ToString(),
                                 FileId = f.Id,
                                 BatchNumber = b.ReferenceNumber,
                                 DocumentNumber = d.ReferenceNumber,
                                 DocumentTypeName = type.Name,
                                 DocumentStatusName = status.Name,
                                 DocumentStepName = step.Name,
                                 CroppedOn = d.CroppedOn,
                                 PageOrder = d.PageOrder,
                                 LastUpdatedOn = d.LastUpdatedOn,
                                 IndexedOn = d.IndexedOn,
                                 ReasonOther = d.ReasonOther,

                                 CreatedBy = d.CreatedBy,
                                 LastStep = d.LastStep,
                                 ModifiedBy = d.ModifiedBy,
                                 ModifiedOn = d.ModifiedOn,
                                 QualityBy = d.QualityBy,
                                 QualityOn = d.QualityOn,

                                 RescanBy = d.RescanBy,
                                 RescanOn = d.RescanOn,
                                 RejectedBy = d.RejectedBy,
                                 RejectedOn = d.RejectedOn,

                                 IndexedBy = d.IndexedBy,
                                 CroppedBy = d.CroppedBy,
                                 SanityOn = d.SanityOn,
                                 SanityBy = d.SanityBy,
                                 FinalQualityOn = d.FinalQualityOn,
                                 FinalQualityBy = d.FinalQualityBy,
                                 ReviewedOn = d.ReviewedOn,
                                 ReviewedBy = d.ReviewedBy,
                                 CreatedOn = d.CreatedOn,
                                 FileTypeId = b.File.FileTypeId
                                 //IndexModels = allindex

                             };


                 var result = await query.Distinct().OrderBy(i => i.PageOrder).ToListAsync();

                 var results = result
                 .GroupBy(x => x.FileId) // Grouper par FileId
                 .Select(group =>
                 {
                     var isSingle = group.First().FileTypeId == 1;
                     return isSingle
                         ? group // Récupérer tous les documents si single
                         : group.Take(1); // Récupérer un seul document si multi
                 })
                 .SelectMany(x => x) // Flatten pour obtenir une liste à plat
                 .ToList();*/

                var batch = (
                    from b in exorabilisContext.Batch
                    join file in exorabilisContext.Files on b.Id equals file.BatchId
                    where b.Id == batchId
                    select file).FirstOrDefault();


                int i = 0;

                // muliple
                if (batch.FileTypeId == 2)
                {
                    foreach (var item in orderData)
                    {

                        var document = exorabilisContext.Document
                               .FirstOrDefault(d => d.Id == item.idDOC);

                        if (document != null)
                        {
                            var multiDocuments = exorabilisContext.Document.Where(x => x.FileId == document.FileId).ToList();

                            if (multiDocuments != null)
                            {
                                foreach (var tempmultiDocument in multiDocuments)
                                {
                                    //var tempmultiDocument = SetActionDoneBy(multiDocument, UserName);

                                    tempmultiDocument.PageOrder = i;

                                    i++;
                                }
                            }
                        }



                    }
                }
                else // single
                {
                    foreach (var item in orderData)
                    {

                        var document = exorabilisContext.Document
                               .FirstOrDefault(d => d.Id == item.idDOC);

                        if (document != null)
                        {
                            document.PageOrder = i; // Assure-toi que `PageOrder` est bien la colonne qui gère l'ordre

                        }

                        i++;

                    }
                }

                exorabilisContext.SaveChanges();
                transaction.Commit();



                var response = new SaveIndexResponse
                {
                    indexdocument = 0,
                    isReturn = 1
                };

                return response;


            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }

        public async Task<int> DeletePages(bool isRecto, long idVerso, long idRecto)
        {
            var transaction = exorabilisContext.Database.BeginTransaction();
            var doctypeid = interfaceBatchDataProvider.GetValueParam("DefaultDocType").Value;
            try
            {
                int retour = 0; // 1 : show next
                var side = isRecto ? "RECTO" : "VERSO";
                var image = new Image();

                if (isRecto)
                {
                    image = exorabilisContext.Image.Where(x => x.Id == idRecto).FirstOrDefault();
                }
                else
                {
                    image = exorabilisContext.Image.Where(x => x.Id == idVerso).FirstOrDefault();

                }

                if (image != null)
                {


                    image.Status = 0;

                    exorabilisContext.SaveChanges();

                    var checkimage = exorabilisContext.Image.FirstOrDefault(x => x.DocumentId == image.DocumentId && x.Status == 1);


                    if (checkimage == null)
                    {
                        var doc = this.exorabilisContext.Document.FirstOrDefault(x => x.Id == image.DocumentId);
                        doc.DocumentStatusId = 6;
                        doc.RejectionCodeId = 7;
                        doc.ReasonOther = "Sheet deleted ";
                        exorabilisContext.SaveChanges();
                        var batchid = this.exorabilisContext.Batch.FirstOrDefault(x => x.Id == doc.BatchId);
                        batchid.NumberOfDocument = this.exorabilisContext.Document.Where(x => x.BatchId == doc.BatchId && x.DocumentTypeId == Convert.ToInt64(doctypeid) && (x.DocumentStatusId == 1 || x.DocumentStatusId == 4)).Count();
                        exorabilisContext.SaveChanges();
                        var otherdoc = this.exorabilisContext.Document.FirstOrDefault(x => x.BatchId == doc.BatchId && (x.DocumentStatusId != 5 && x.DocumentStatusId != 6) && x.DocumentTypeId == Convert.ToInt64(doctypeid));

                        if (otherdoc == null)
                        {

                            batchid.DocumentStatusId = 5;
                            batchid.RejectionCodeId = 7;
                            batchid.NumberOfDocument = this.exorabilisContext.Document.Where(x => x.BatchId == doc.BatchId && x.DocumentTypeId == Convert.ToInt64(doctypeid)).Count();
                            batchid.ReasonOther = "batch deleted cause of sheet deleted";
                            exorabilisContext.SaveChanges();
                        }

                        retour = 1;
                    }


                }

                transaction.Commit();

                return retour;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(ex.Message);

            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }

        public List<TrademarkWithPagesViewModel> GetAllTrademarksWithPages(string pdfFolderPath)
        {
            // Récupérer tous les fichiers PDF dans le dossier et sous-dossiers spécifiés
            var pdfFiles = Directory.GetFiles(pdfFolderPath, "*.pdf", SearchOption.AllDirectories);

            // Dictionnaire pour stocker le nombre de pages de chaque PDF par file_nbr
            var fileNbrToPageCount = new Dictionary<int, int>();

            foreach (var filePath in pdfFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);  // Exemple: "MU-M-2021-23234"
                var parts = fileName.Split('-');
                if (parts.Length < 4) continue;

                var fileNbr = int.Parse(parts[3]);  // Extraire le numéro du fichier (ex: 23234)

                //var existingPolice = this.exorabilisContext.PoliceIndex
                //.FirstOrDefault(p => p.batchnumber == fileNbr.ToString());

                //if (existingPolice == null)
                //{
                //    // Ajouter la nouvelle entité seulement si elle n'existe pas déjà
                //    var police = new PoliceIndex
                //    {
                //        batchnumber = fileNbr.ToString()
                //    };

                //    this.exorabilisContext.PoliceIndex.Add(police);


                //}
                // Ajouter le nombre de pages pour ce file_nbr
                var pageCount = GetNumberOfPagesForFile(filePath);
                if (!fileNbrToPageCount.ContainsKey(fileNbr))
                {
                    fileNbrToPageCount.Add(fileNbr, pageCount);
                }
            }

            //this.exorabilisContext.SaveChanges();

            // Récupérer toutes les données de la base de données
            var trademarks = this.exorabilisContext.Trademarks.ToList();

            var trademarksWithPages = new List<TrademarkWithPagesViewModel>();
            int i = 1;
            foreach (var trademark in trademarks)
            {


                if (fileNbrToPageCount.ContainsKey(trademark.file_nbr))
                {

                    // Ajouter le nombre de pages dans le résultat
                    var trademarkWithPages = new TrademarkWithPagesViewModel
                    {
                        id = i,
                        file_seq = trademark.file_seq,
                        file_typ = trademark.file_typ,
                        file_ser = trademark.file_ser,
                        file_nbr = trademark.file_nbr,
                        mark_name = trademark.mark_name,
                        filing_date_string = trademark.filing_date_string,
                        filing_date = trademark.filing_date,
                        NumberOfPages = fileNbrToPageCount[trademark.file_nbr]
                    };

                    trademarksWithPages.Add(trademarkWithPages);
                    i++;
                }

            }

            return trademarksWithPages.OrderBy(x => x.file_nbr).ToList();
        }

        // Méthode pour récupérer le nombre de pages d'un fichier PDF avec PdfSharp
        private int GetNumberOfPagesForFile(string filePath)
        {
            try
            {
                // Ouvrir le fichier PDF en mode lecture
                using (PdfDocument document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
                {
                    // Retourner le nombre de pages
                    return document.PageCount;
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, loguer l'erreur et retourner 0
                Console.WriteLine($"Erreur de lecture du fichier {filePath}: {ex.Message}");
                return 0;
            }
        }

        public async Task upload_excel(IFormFile file)
        {
            var transaction = this.exorabilisContext.Database.BeginTransaction();

            try
            {
                if (file == null || file.Length == 0)
                    throw new Exception("Fichier manquant");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var projects = new List<Project>();
                var projects_type = new List<Document_type>();
                var projects_folio = new List<ProjectFolio>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        foreach (var ws in package.Workbook.Worksheets)
                        {
                            int rowHeader = 3;
                            int startDataRow = 5;
                            int lastRow = ws.Dimension.End.Row;
                            int maxCol = ws.Dimension.End.Column;

                            for (int row = startDataRow; row <= lastRow; row++)
                            {
                                string reference = ws.Cells[row, 1].Text;
                                string name = ws.Cells[row, 2].Text;


                                if (string.IsNullOrWhiteSpace(reference) && string.IsNullOrWhiteSpace(name))
                                    continue; // ligne vide

                                Project verifProject = this.exorabilisContext.Project.FirstOrDefault(x => x.Reference == reference);
                                long projectId = 0;

                                if (verifProject == null)
                                {
                                    var newProject = new Project { Name = name, Reference = reference };
                                    this.exorabilisContext.Project.Add(newProject);
                                    this.exorabilisContext.SaveChanges();
                                    projectId = newProject.Id;
                                }
                                else
                                {
                                    projectId = verifProject.Id;
                                }


                                for (int col = 3; col <= maxCol; col += 2)
                                {
                                    var type = ws.Cells[rowHeader, col].Text;
                                    var foliosText = ws.Cells[row, col].Text;
                                    var pagesText = ws.Cells[row, col + 1].Text;


                                    if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(foliosText)) continue;

                                    Document_type verifDocumentType = this.exorabilisContext.Document_type.FirstOrDefault(x => x.Name == type.ToString());

                                    long projectTypeId = 0;

                                    if (verifDocumentType == null)
                                    {
                                        var newProjectType = new Document_type { Name = type.ToString() };
                                        exorabilisContext.Document_type.Add(newProjectType);
                                        exorabilisContext.SaveChanges();
                                        projectTypeId = newProjectType.Id;
                                    }
                                    else
                                    {
                                        projectTypeId = verifDocumentType.Id;
                                    }

                                    var foliosList = foliosText.ToString().Split(new[] { ',' , '&' , '.' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Where(f => !string.IsNullOrWhiteSpace(f))
                                        .Select(f => f.Trim())
                                                            .ToList();

                                    var pagesList = pagesText.ToString().Split(new[] { ',', '&', '.' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Where(f => !string.IsNullOrWhiteSpace(f))

                                                             .Select(p => p.Trim())
                                                             .ToList();

                                    List<(string Folio, string Page)> folioPagePairs = new List<(string, string)>();

                                    if (pagesList.Count == 1)
                                    {
                                        // Une seule page pour tous les folios
                                        foreach (var folio in foliosList)
                                        {
                                            folioPagePairs.Add((folio, pagesList[0]));
                                        }
                                    }
                                    else if (foliosList.Count == pagesList.Count)
                                    {
                                        // Correspondance 1 à 1
                                        for (int i = 0; i < foliosList.Count; i++)
                                        {
                                            folioPagePairs.Add((foliosList[i], pagesList[i]));
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Le nombre de pages ne correspond pas au nombre de folios, et il y a plus d'une page.");
                                    }


                                    foreach (var item in folioPagePairs)
                                    {

                                        ProjectFolio verifProjectFolio = this.exorabilisContext.Project_folio
                                        .FirstOrDefault(x => x.Project_id == projectId && x.Document_type_id == projectTypeId
                                        && x.NbrOfPages == Convert.ToInt64(item.Page) && x.Folios == item.Folio);

                                        if (verifProjectFolio == null)
                                        {
                                            var newProjectFolio = new ProjectFolio
                                            {
                                                Project_id = projectId,
                                                Document_type_id = projectTypeId,
                                                Folios = item.Folio,
                                                NbrOfPages = long.TryParse(item.Page, out long p) ? p : (int?)0
                                            };
                                            exorabilisContext.Project_folio.Add(newProjectFolio);
                                            exorabilisContext.SaveChanges();
                                        }

                                    }


                                }

                            }
                        }
                    }

                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(ex.StackTrace + ex.Message);
            }
            finally
            {
                transaction.Dispose(); // Libération des ressources
            }
        }
    }
}