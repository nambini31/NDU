using System.Collections.Generic;

namespace Core.Entity.MyCore
{
    public class Element
    {
        public Element()
        {
            ElementHierarchyElement = new HashSet<ElementHierarchy>();
            ElementHierarchyParentElement = new HashSet<ElementHierarchy>();
            RoleAccessControl = new HashSet<RoleAccessControl>();
        }

        public string Id { get; set; }
        public long MyId { get; set; }
        public int? ElementNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultDisplayText { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string Url { get; set; }
        public int Status { get; set; }
        public int? Step { get; set; }
        public int? Order { get; set; }

        public virtual ICollection<ElementHierarchy> ElementHierarchyElement { get; set; }
        public virtual ICollection<ElementHierarchy> ElementHierarchyParentElement { get; set; }
        public virtual ICollection<RoleAccessControl> RoleAccessControl { get; set; }
    }
}
