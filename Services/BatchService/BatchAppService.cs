
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.RequestPostGet;
using Core.ViewModel.Exorabilis;
using DataProviders.BatchProvider;

namespace Services.BatchService
{
    public class BatchAppService : InterfaceBatchAppService
    {
        private readonly InterfaceBatchDataProvider batchDataProvider;   
        public BatchAppService(InterfaceBatchDataProvider _batchDataProvider)
        {
            this.batchDataProvider = _batchDataProvider;
        }
        public async Task<BatchAndCodeRejectViewModel> getAllBatchByStep(BatchSearchRequest request)
        {
            var data = new BatchAndCodeRejectViewModel {
                Batch = await this.batchDataProvider.getAllBatchByStep(request),
                RejectionCodeAll =  await this.batchDataProvider.getAllRejectionCode()
            };

            return data;
        } 
        
        public async Task excel_for_batch(BatchSearchRequest request)
        {
            await this.batchDataProvider.excel_for_batch(request);
        }

        public async Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchById(string? batchId)
        {
            return  await this.getBatchById(batchId);
        }

        public Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchByRef(string batchReference)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, string)> IsBatchLocked(BatchwithStepRequest batchId, string username)
        {
             return await this.batchDataProvider.IsBatchLocked(batchId, username);
        } 
        
        public async Task<bool> MakeCompletedBatch(BatchwithStepRequest batchId)
        {
             return await this.batchDataProvider.MakeCompletedBatch(batchId);
        }
        public bool Export(BatchSearchRequest request)
        {
            return this.batchDataProvider.Export(request);
        }
        public async Task LockBatch(string batchId, string username)
        {
             await this.batchDataProvider.LockBatch(batchId , username);
        }

        public async Task<IEnumerable<RejectionCode>> getAllRejectionCode()
        {
            return await this.batchDataProvider.getAllRejectionCode();        }

        public async Task<bool> RejectBatch(string batchId, long rejectionId, string UserId, string UserName, string otherreason)
        {
            return await this.batchDataProvider.RejectBatch(batchId, rejectionId, UserId, UserName , otherreason);  
        }

        public Parameter GetValueParam(string name)
        {
           return this.batchDataProvider.GetValueParam(name);
        }
    }
}
