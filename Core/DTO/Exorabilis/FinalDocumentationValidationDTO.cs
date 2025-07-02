using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO.Exorabilis
{
    public class FinalDocumentationValidationDTO
    {
        public string? Id { get; set; }
        public DateTime ScannedOn { get; set; }
        public int TotalDocument { get; set; }
        public int TotalToBeValidated { get; set; }
        public int TotalValidated { get; set; }
        public int PercentageValidated { get; set; }

        public int TotalAccepted { get; set; }
        public int TotalRejected { get; set; }
        public decimal SuccessRate { get; set; }
        public string? ValidatedBy { get; set; }

        public int IsLocked { get; set; }
        public int IsCompleted { get; set; }
        public int IsAccepted { get; set; }
        public int IsRejected { get; set; }
        public string? FromLicense { get; set; }
        public string? ToLicense { get; set; }
        public string? FromLicenseReal { get; set; }
        public string? ToLicenseReal { get; set; }
    }
}
