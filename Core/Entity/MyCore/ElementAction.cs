namespace Core.Entity.MyCore
{
    public class ElementAction
    {
        public string Id { get; set; }
        public long MyId { get; set; }
        public string ElementId { get; set; }
        public string ActionName { get; set; }
        public string Controller { get; set; }
        public string RouteName { get; set; }
        public string Url { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }
    }
}
