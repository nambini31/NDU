using Core.DTO.Exorabilis;
using Core.RequestPostGet;
using Core.ViewModel.Exorabilis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DocumentService
{
    public interface InterfaceDocumentAppService
    {
        public Task<BatchAndDocumentViewModel> getAllDocumentsByBatchWithStep(BatchwithStepRequest batchwithStep);
        public Task<BatchAndDocumentViewModel> documentinbatchAsync(BatchwithStepRequest batchwithStep);
        public Task<BatchAndDocumentViewModel> documentinbatchOrder(BatchwithStepRequest batchwithStep);
        public Task<IndexDocumentWithImageViewModel> getImagesOfNextDocumentsIFMultiByDocIdNext(string docId  , string referenceNumber, long fileTypeId, string nextorprev , long step);
        public Task<int> DeletePages(bool isRecto, long idVerso, long idRecto);
        public Task<IndexDocumentWithImageViewModel> getAllImageWithDocumentByDocId(string docId);
        public Task<Dashboard_DTO> Dashboard(DocumentSearchRequest request);
        public Task<bool> SaveAllDocumentsReview(DocumentSearchRequest request);
        public Task<bool> RescanAllDocumentsReview(DocumentSearchRequest request);
        public IEnumerable<DocumentIndex_DTO> getAllIndexList(string docId);
        public Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList();
        public List<TrademarkWithPagesViewModel> GetAllTrademarksWithPages(string pdfFolderPath);
        public Task upload_excel(IFormFile excelFile);
        public Task<BatchAndDocumentViewModel> getAllDocumentsReview(DocumentSearchRequest request);
        public Task<SaveIndexResponse> SaveIndexation(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev , string otherreason, string step);
        public Task<SaveIndexResponse> SaveModification(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev);
        public Task<SaveIndexResponse> Saveordereddocuments(long batchId, List<ValueOrderRequest> orderData);

        public Task<string> SaveImageCropped(IConfiguration _configuration, IWebHostEnvironment webhostEnvironment, string docId , string BatchId, string documentnumber, string Image, bool isRecto, string UserId, string UserName);
    }
}
