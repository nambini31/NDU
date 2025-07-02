using Core.DTO.Exorabilis;
using Core.Entity.Exorabilis;
using Core.Entity.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.Exorabilis
{
    public class BatchAndDocumentViewModel
    {
        public IEnumerable<DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO>? doccument { get; set; }
        public Batch_Quality_Crop_Index_Sanity_Unlock_DTO? Batch { get; set; }
        public IEnumerable<DocumentIndex_DTO>? DocumentIndexAll { get; set; }
        public IEnumerable<RejectionCode>? RejectionCodeAll { get; set; }
        public string? imageextension { get; set; }

        public DocumentForBatch_Quality_Crop_Index_Sanity_Unlock_DTO? firstIdDocEncrypt {get; set;}

        public List<ElementBoutton>? ElemBoutton { get; set; }

    }
}
