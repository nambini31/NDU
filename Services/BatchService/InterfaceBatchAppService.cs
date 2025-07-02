
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.RequestPostGet;
using Core.ViewModel.Exorabilis;

namespace Services.BatchService
{
    public interface InterfaceBatchAppService
    {
        public Task LockBatch(string batchId, string username);
        public Task<bool> MakeCompletedBatch(BatchwithStepRequest request);
        public Task<(bool , string)> IsBatchLocked(BatchwithStepRequest batchId, string username);
        public bool Export(BatchSearchRequest request);
        public Parameter GetValueParam(string name);
        public Task<IEnumerable<RejectionCode>> getAllRejectionCode();
        public Task<bool> RejectBatch(string batchId, long rejectionId, string UserId, string UserName , string otherreason);
        public Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchById(string? batchId);
        public Task<Batch_Quality_Crop_Index_Sanity_Unlock_DTO> getBatchByRef(string batchReference);
        public Task<BatchAndCodeRejectViewModel> getAllBatchByStep(BatchSearchRequest request);
        public Task excel_for_batch(BatchSearchRequest request);
    }
}
