using Core.Database;
using Core.Entity.MyCore;
using System.Linq;

namespace Core.UserDataProviders.Role
{
    public class RoleDataProvider : IRoleDataProvider
    {
        private readonly MyCoreContext _context;

        public RoleDataProvider(MyCoreContext context)
        {
            _context = context;
        }

        public Core.Entity.MyCore.Role GetRoleById(string? roleId)
        {
            try
            {
                return _context.Role.Find(roleId);
            }
            catch
            {
                return null;
            }
        }

        public UserRole? GetUserRoleByUserId(string? userId)
        {
            try
            {
                return _context.UserRole.FirstOrDefault(x => x.UserId==userId && x.Status.Equals(1));
            }
            catch
            {
                return null;
            }
        }
    }
}