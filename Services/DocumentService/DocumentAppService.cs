using Core.Database;
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.RequestPostGet;
using Core.ServiceEncryptor;
using Core.ViewModel.Exorabilis;
using DataProviders.BatchProvider;
using DataProviders.DocumentProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.DocumentService
{
    public class DocumentAppService : InterfaceDocumentAppService
    {
        private readonly InterfaceDocumentDataProvider interfaceDocumentDataProvider;
        private readonly InterfaceBatchDataProvider interfaceBatchDataProvider;

        public async Task<BatchAndDocumentViewModel> documentinbatchAsync (BatchwithStepRequest batchwithStep )
        {
            var data = new BatchAndDocumentViewModel
            {
                doccument = await this.interfaceDocumentDataProvider.getAllDocumentsByBatchWithStep(batchwithStep),
                firstIdDocEncrypt = await this.interfaceDocumentDataProvider.getFirstDocument(batchwithStep)
            };

            return data;
        }
        public async Task<BatchAndDocumentViewModel> documentinbatchOrder (BatchwithStepRequest batchwithStep )
        {
            var data = new BatchAndDocumentViewModel
            {
                doccument = await this.interfaceDocumentDataProvider.getAllDocumentsByBatchToOrder(batchwithStep),
            };

            return data;
        }

        public DocumentAppService(InterfaceDocumentDataProvider interfacedoc , InterfaceBatchDataProvider interfaceBatchData)
        {
            interfaceDocumentDataProvider = interfacedoc;
            interfaceBatchDataProvider = interfaceBatchData;
        }
        public async Task<BatchAndDocumentViewModel> getAllDocumentsByBatchWithStep(BatchwithStepRequest batchwithStep)
        {
            var data = new BatchAndDocumentViewModel
            {
                Batch = await this.interfaceBatchDataProvider.getBatchById(batchwithStep.batchId),
                firstIdDocEncrypt = await this.interfaceDocumentDataProvider.getFirstDocument(batchwithStep),
                RejectionCodeAll = await this.interfaceBatchDataProvider.getAllRejectionCode()
            };

            return data;

        }


        public async Task<BatchAndDocumentViewModel> getAllDocumentsReview(DocumentSearchRequest request)
        {
            var data = new BatchAndDocumentViewModel
            {
                DocumentIndexAll =  await this.interfaceDocumentDataProvider.getAllIndexList(),
                doccument = await this.interfaceDocumentDataProvider.getAllDocumentsReview(request),
                RejectionCodeAll = await this.interfaceBatchDataProvider.getAllRejectionCode()
            };
            return data;
        }

        public async Task<IndexDocumentWithImageViewModel> getAllImageWithDocumentByDocId(string docId)
        {
            var chrono = await interfaceBatchDataProvider.getChrono();
            var data = new IndexDocumentWithImageViewModel
            {
                doccumentImage = await this.interfaceDocumentDataProvider.getAllImageByDocId(docId),
                doccumentIndex = new DocumentIndexFile_DTO
                {
                    docsDTO = this.interfaceDocumentDataProvider.getAllIndexList(docId),
                },


                IsPath = chrono.DatabaseSaveImage.ToString()
            };

            data.doccumentIndex.FileId = data.doccumentImage.FileId;

            return data;
        }

        public IEnumerable<DocumentIndex_DTO> getAllIndexList(string docId)
        {
            return  this.interfaceDocumentDataProvider.getAllIndexList(docId);
        }
        public async Task<IEnumerable<DocumentIndex_DTO>> getAllIndexList()
        {
            return  await this.interfaceDocumentDataProvider.getAllIndexList();
        }

        public async Task<IndexDocumentWithImageViewModel> getImagesOfNextDocumentsIFMultiByDocIdNext(string docId, string referenceNumber, long fileTypeId, string nextorprev, long step)
        {
            var chrono = await interfaceBatchDataProvider.getChrono();

            var data = new IndexDocumentWithImageViewModel
            {
                IsPath = chrono.DatabaseSaveImage.ToString(),
                doccumentImage = await this.interfaceDocumentDataProvider.getImagesOfNextDocumentsIFMultiByDocIdNext(docId, referenceNumber, fileTypeId ,nextorprev ,step)
            };

            return data;
        }

        public async Task<bool> SaveAllDocumentsReview(DocumentSearchRequest request)
        {
            return await this.interfaceDocumentDataProvider.SaveAllDocumentsReview(request);
        }
        
        public async Task<bool> RescanAllDocumentsReview(DocumentSearchRequest request)
        {
            return await this.interfaceDocumentDataProvider.RescanAllDocumentsReview(request);
        }

        public async Task<string> SaveImageCropped(IConfiguration _configuration, IWebHostEnvironment webhostEnvironment, string docId, string BatchId, string documentnumber, string Image, bool isRecto, string UserId, string UserName)
        {
            var data = await this.interfaceDocumentDataProvider.SaveImageCropped(_configuration , webhostEnvironment , docId , BatchId, documentnumber , Image , isRecto, UserId, UserName);

            return data;
        }

        public Task<SaveIndexResponse> SaveIndexation(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev , string otherraison , string step)
        {
            return this.interfaceDocumentDataProvider.SaveIndexation(docId, referenceNumber , fileTypeId , UserId , UserName , reason , type , Project_folio_id, nextorprev , otherraison , step);
        }
        public Task<SaveIndexResponse> SaveModification(string docId, string referenceNumber, long fileTypeId, string UserId, string UserName, long? reason, long type, long Project_folio_id, string nextorprev)
        {
            return this.interfaceDocumentDataProvider.SaveModification(docId, referenceNumber , fileTypeId , UserId , UserName , reason , type , Project_folio_id, nextorprev);
        }

        public async Task<Dashboard_DTO> Dashboard(DocumentSearchRequest request)
        {
            return await this.interfaceDocumentDataProvider.Dashboard(request);
        }

        public Task<SaveIndexResponse> Saveordereddocuments(long batchId, List<ValueOrderRequest> orderData)
        {
            return this.interfaceDocumentDataProvider.Saveordereddocuments(batchId, orderData);
        }

        public async Task<int> DeletePages(bool isRecto, long idVerso, long idRecto)
        {
            return await this.interfaceDocumentDataProvider.DeletePages(isRecto, idVerso, idRecto);
        }

        public List<TrademarkWithPagesViewModel> GetAllTrademarksWithPages(string pdfFolderPath)
        {
            return this.interfaceDocumentDataProvider.GetAllTrademarksWithPages(pdfFolderPath);
        }

        public Task upload_excel(IFormFile excelFile)
        {
            return this.interfaceDocumentDataProvider.upload_excel(excelFile);
        }
    }
}
