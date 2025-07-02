using System.Collections.Generic;
using Core.Entity.MyCore;
using Core.UserModels;
using Core.ViewModel;

namespace Core.UserServices.Role
{
    public interface IRoleService
    {
        bool AddRole(RoleModel model);
        bool UpdateRole(RoleModel model);
        bool DeleteRole(string id);
        bool AsignRoleToUser(UserRoleModel model);
        bool AsignRoleToUsers(ICollection<UserRoleModel> models);
        bool UpdateUserRole(UserRoleModel model);
        List<RoleModel> GetAllRoles();
        List<ElementModel> GetAllElements();
        List<ElementBoutton> GetElementBouttons();
        bool UpdateRoleAccess(string roleId, IEnumerable<string> menu);

        bool UpdateAccessButton(string MyId, IEnumerable<string> selectedBtn, IEnumerable<string> notSelectedBtn);
        List<string> GetRoleAccessibleMenu(string roleId);
        List<int> GetAccesBoutton(string MyId);
        bool CanUserAccessLink(string userId, string url);
        bool IsAdmin(string roleId);
        List<ElementModel> GetElementsByRoleId(string roleId);
        List<ElementBoutton> GetElementsBouttonByStep(int step);

    }
}