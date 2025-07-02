using Core.Entity.MyCore;
using Core.UserModels;
using Core.ViewModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.UserDataProviders.Users
{
    public interface IUserDataProvider
    {
        User AuthenticateUser(LoginModel model);
        Person GetPersonById(string personId);
        string SaveUser(CreateUserModel model);
        //User GetUserInfo(ResetPasswordModel model);
        //UserStatus GetUserStatusByLibelle(string libelle);
        List<Person> GetUsers(string keyword, int page, int pageSize);
        //List<Person> GetUsers();
        //User GetUserByPasswordRequest(string request);
        //bool UpdateUserInfo(ConfirmResetPasswordModel model);
        //bool IsMailExist(string mail);
        //bool IsUserNameExist(string username);
        User GetUserById(string userId);
        bool UpdateUser(CreateUserModel model);
        int CountUsers(string keyword);
        User GetUserByPersonId(string personId);
        bool DeleteUser(string userId);
        //List<User> GetWmaUsers(string memberAndStatus);
        //List<User> GetREDERUsers();
    }
}
