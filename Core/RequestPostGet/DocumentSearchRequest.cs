namespace Core.RequestPostGet
{
    public class DocumentSearchRequest
    {
        public DateOnly? firstdate { get; set; }
        public DateOnly? lastdate { get; set; }
        public string? batchnumber { get; set; }
        public string? documentnumber { get; set; }
        public string? userId { get; set; }
        public string? UserName { get; set; }
        public string? otherreason { get; set; }
        public long? reason { get; set; }
        public long? step { get; set; }
    }
}
