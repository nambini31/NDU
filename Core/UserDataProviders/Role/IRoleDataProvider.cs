
using Core.Entity.MyCore;

namespace Core.UserDataProviders.Role
{
    public interface IRoleDataProvider
    {
        Core.Entity.MyCore.Role GetRoleById(string? roleId);
        UserRole? GetUserRoleByUserId(string userId);
    }
}