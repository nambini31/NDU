using Core.UserModels;
using Core.ViewModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UserServices.Users
{
    public interface IUserService
    {
        UserModel? AuthenticateUser(LoginModel model);

        void CloseSession(UserModel model);
        bool SaveUser(CreateUserModel model);
        bool UpdateUser(CreateUserModel model);
        string SaveUserFromApi(CreateUserModel model);
        UserModel GetUserInfo(ResetPasswordModel model);
        ConfirmResetPasswordModel GetUserByPasswordRequest(string request);
        bool UpdateUserInfo(ConfirmResetPasswordModel model);
        bool IsMailExist(string mail);
        CreateUserModel GetUserById(string userId);
        List<CreateUserModel> GetUsers(string? keyword, int page, int pageSize);
        List<CreateUserModel> GetUsers();
        int CountUsers(string? keyword);
        bool DeleteUser(string userId);

        string? GetUserRole(string userId);
    }
}
