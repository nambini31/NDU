namespace Core.Entity.MyCore
{
    public class VersionHistoryQueries
    {
        public string Id { get; set; }
        public long? MyId { get; set; }
        public string VersionHistoryId { get; set; }
        public string Query { get; set; }
        public int? Status { get; set; }

        public virtual VersionHistory VersionHistory { get; set; }
    }
}
