using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
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

namespace DataProviders.DocumentProvider
{
    public interface InterfaceDocumentDataProvider
    {
        public Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsByBatchWithStep(BatchwithStepRequest batchwithStep);
        public Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsByBatchToOrder(BatchwithStepRequest batchwithStep);
        public Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList(string docId);
        public Task<int> DeletePages(bool isRecto, long idVerso, long idRecto);
        public Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList();
        public Task<Dashboard_DTO> Dashboard(DocumentSearchRequest request);
        public List<TrademarkWithPagesViewModel> GetAllTrademarksWithPages(string pdfFolderPath);
        public Task upload_excel(IFormFile excelFile);
        public Task<bool> SaveAllDocumentsReview(DocumentSearchRequest request);
        public Task<bool> RescanAllDocumentsReview(DocumentSearchRequest request);
        public Task<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO> getFirstDocument(BatchwithStepRequest batchwithStep);
        public Task<DocumentImage_DTO> getAllImageByDocId(string docId);
        public Task<SaveIndexResponse> SaveIndexation(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_type_value_id, string nextorprev , string otherreason , string step);
        public Task<SaveIndexResponse> SaveModification(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_type_value_id, string nextorprev);
        public Document SetActionDoneBy(Document document, string userName);
        public Task<IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllDocumentsReview(DocumentSearchRequest request);
        public long GetNextStepId(long currentStepId);
        public Task<SaveIndexResponse> Saveordereddocuments(long batchId, List<ValueOrderRequest> orderData);
        public Task<DocumentImage_DTO> getImagesOfNextDocumentsIFMultiByDocIdNext(string docId,string referenceNumber, long fileTypeId, string nextorprev , long step);
        public Task<string> SaveImageCropped(IConfiguration _configuration, IWebHostEnvironment webhostEnvironment, string docId, string BatchId, string documentnumber, string Image, bool isRecto, string UserId, string UserName);
    }
}
