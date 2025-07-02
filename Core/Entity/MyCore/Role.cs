using System.Collections.Generic;

namespace Core.Entity.MyCore
{
    public class Role
    {
        public Role()
        {
            RoleAccessControl = new HashSet<RoleAccessControl>();
            RoleHierarchyParentRole = new HashSet<RoleHierarchy>();
            RoleHierarchyRole = new HashSet<RoleHierarchy>();
            UserRole = new HashSet<UserRole>();
        }

        public string Id { get; set; }
        public long MyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LastUpdatedBy { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<RoleAccessControl> RoleAccessControl { get; set; }
        public virtual ICollection<RoleHierarchy> RoleHierarchyParentRole { get; set; }
        public virtual ICollection<RoleHierarchy> RoleHierarchyRole { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
