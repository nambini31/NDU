namespace Core.Entity.MyCore
{
    public class ElementType
    {
        public string Id { get; set; }
        public long MyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int? Status { get; set; }
    }
}
