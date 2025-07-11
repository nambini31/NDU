
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.RequestPostGet;

namespace DataProviders.BatchProvider
{
    public interface InterfaceBatchDataProvider
    {
        public Task LockBatch(string batchId, string username);
        public Task<(bool, string)> IsBatchLocked(BatchwithStepRequest batchId , string username);
        public Task<IEnumerable<RejectionCode>> getAllRejectionCode();
        public Task<bool> MakeCompletedBatch(BatchwithStepRequest batchId);
        public Parameter GetValueParam(string name);
        public bool Export(BatchSearchRequest request);
        public Batch SetActionDoneByBatch(Batch document, string userName);
        public Task<Chrono> getChrono();
        public void createOrUpdateBatchHistory(Batch batch, string userId, string otherreason);
        public Task<bool> RejectBatch(string batchId, long rejectionId, string UserId, string UserName, string otherreason);
        public Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchById(string? batchId);
        public Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchByRef(string batchReference);
        public Task<IEnumerable<Batch_Quality_Crop_Index_Sanity_Unlock_DTO>> getAllBatchByStep(BatchSearchRequest request);
        public Task excel_for_batch(BatchSearchRequest request);
    }
}
