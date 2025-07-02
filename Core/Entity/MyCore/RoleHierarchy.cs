namespace Core.Entity.MyCore
{
    public class RoleHierarchy
    {
        public string Id { get; set; }
        public long MyId { get; set; }
        public string RoleId { get; set; }
        public string ParentRoleId { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int Status { get; set; }

        public virtual Role ParentRole { get; set; }
        public virtual Role Role { get; set; }
    }
}
