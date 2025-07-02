namespace Core.Entity.MyCore
{
    public class Address
    {
        public string Id { get; set; }
        public long MyId { get; set; }
        public string AddressTypeId { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string TownCity { get; set; }
        public string CountryId { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }

        public virtual Country Country { get; set; }
    }
}
