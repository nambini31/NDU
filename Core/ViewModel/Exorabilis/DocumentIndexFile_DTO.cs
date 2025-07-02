using Core.DTO.Exorabilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.Exorabilis
{
    public class DocumentIndexFile_DTO
    {
        public IEnumerable<DocumentIndex_DTO>? docsDTO { get; set; }
        public long? FileId { get; set; }
    }
}
