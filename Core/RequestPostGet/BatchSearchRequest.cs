using Microsoft.AspNetCore.Hosting;

namespace Core.RequestPostGet
{
    public class BatchSearchRequest
    {
        public DateOnly? firstdate { get; set; }
        public DateOnly? lastdate { get; set; }
        public string? batchnumber { get; set; }
        public long? step { get; set; }
        public IWebHostEnvironment? webhostEnvironment { get; set; }
    }
}
