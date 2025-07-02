using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO.Exorabilis
{
    public class DocumentImage_DTO
    {
        public string? IdRecto { get; set; }
        public string? IdVerso { get; set; }
        public string? DocumentNumber { get; set; }
        public string? otherreason { get; set; }
        public long? rejectioncodeid { get; set; }
        public long? DocumentStep { get; set; }
        public long? DocumentStatus { get; set; }
        public int? DocsCount { get; set; }
        public string? IdEncrypt { get; set; }
        public string? PathRecto { get; set; }
        public string? FileNumber { get; set; }
        public bool? isQualited { get; set; }
        public string? ImageContentStr { get; set; }
        public string? PathVerso { get; set; }
        public string? ImageVersoContentStr { get; set; }
        public long? FileTypeId { get; set; }
        public long? FileId { get; set; }
    }
}
