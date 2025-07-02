using Core.Entity.MyCore;
using Core.ViewModel;
using System.Collections.Generic;

namespace Core.UserModels
{
    public class RoleAccessControlModel
    {
        public List<RoleModel> Roles { get; set; }
        public List<ElementModel> ParentElements { get; set; }
        public List<ElementBoutton> ElementsBoutton { get; set; }
    }
}