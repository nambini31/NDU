using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UserModels
{
    public class UserModel
    {
        //User info
        public string Id { get; set; }
        public long MyId { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string HashCode { get; set; }
        [DataType(DataType.Password), MinLength(6)]
        public string ConfirmHashCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UserStatusId { get; set; }
        public string UserRoleId { get; set; }
        public string? UserRoleName { get; set; }
        public UserRoleModel? UserRole { get; set; }
        public string? PasswordResetRequest { get; set; }
        public string Mail { get; set; }

        //Person info
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public string? Title { get; set; }

        public string DisplayName => $"{FirstName} {LastName}";

        public List<RoleModel> Roles { get; set; }
    }
}
