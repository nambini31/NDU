using Core.UserDataProviders.Role;
using Core.UserDataProviders.Session;
using Core.UserDataProviders.Users;
using Core.UserModels;
using Core.ViewModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UserServices.Users
{
    public class UserService : IUserService
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly ISessionDataProvider _sessionDataProvider;
        public readonly IRoleDataProvider _roleDataProvider;

        public UserService(IUserDataProvider userDataProvider, ISessionDataProvider sessionDataProvider, IRoleDataProvider roleDataProvider)
        {
            _userDataProvider = userDataProvider;
            _sessionDataProvider = sessionDataProvider;
            _roleDataProvider = roleDataProvider;
        }
        public UserModel? AuthenticateUser(LoginModel model)
        {
            try
            {
                var user = _userDataProvider.AuthenticateUser(model);
                if (user != null)
                {
                    if (user != null) _sessionDataProvider.SetSession(user.Id, false);
                    var person = _userDataProvider.GetPersonById(user.PersonId);
                    var userRole = _roleDataProvider.GetUserRoleByUserId(user.Id);
                    var role = _roleDataProvider.GetRoleById(userRole?.RoleId);
                    if(person==null|| userRole==null) return null;
                    return new UserModel
                    {
                        Id = user.Id,
                        MyId = user.MyId,
                        UserName = user.UserName,
                        HashCode = user.HashCode,
                        CreatedOn = user.CreatedOn,
                        UserStatusId = user.UserStatusId,
                        Mail = user.Mail,
                        PersonId = person.Id,
                        FirstName = person.FirstName,
                        MiddleName = person.MiddleName,
                        LastName = person.LastName,
                        PasswordResetRequest = user.PasswordResetRequest,
                        UserRoleName = role.Name,
                        UserRoleId = role.Id,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void CloseSession(UserModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model?.Id)) _sessionDataProvider.SetSession(model.Id, true);
            }
            catch
            {
                // ignored
            }
        }

        public bool SaveUser(CreateUserModel model)
        {
            return !string.IsNullOrEmpty(_userDataProvider.SaveUser(model));
        }

        public bool UpdateUser(CreateUserModel model)
        {
            return _userDataProvider.UpdateUser(model);
        }

        public string SaveUserFromApi(CreateUserModel model)
        {
            throw new NotImplementedException();
        }

        public UserModel GetUserInfo(ResetPasswordModel model)
        {
            throw new NotImplementedException();
        }

        public ConfirmResetPasswordModel GetUserByPasswordRequest(string request)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserInfo(ConfirmResetPasswordModel model)
        {
            throw new NotImplementedException();
        }

        public bool IsMailExist(string mail)
        {
            throw new NotImplementedException();
        }

        public CreateUserModel GetUserById(string userId)
        {
            try
            {
                var user = _userDataProvider.GetUserById(userId);
                var person = _userDataProvider.GetPersonById(user.PersonId);
                var userRole = _roleDataProvider.GetUserRoleByUserId(userId);
                return new CreateUserModel
                {
                    Id = user.Id,
                    Identifiant = user.UserName,
                    Prenom = person.FirstName,
                    NomDeFamille = person.LastName,
                    Role = userRole?.RoleId,

                };
            }
            catch (Exception)
            {
                return new CreateUserModel();
            }
        }

        public List<CreateUserModel> GetUsers(string? keyword, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword)) keyword = string.Empty;
                var persons = _userDataProvider.GetUsers(keyword!, page, pageSize);
                var result = (from person in persons
                        let user = _userDataProvider.GetUserByPersonId(person.Id)
                        let userRole = user != null ? _roleDataProvider.GetUserRoleByUserId(user.Id) : null
                        let role = userRole != null ? _roleDataProvider.GetRoleById(userRole.RoleId) : null
                        where user != null
                        select new CreateUserModel
                        {
                            Id = user.Id,
                            Identifiant = user.UserName,
                            Role = role?.Name,
                            UserRoleId = role?.Id,
                            Prenom = person.FirstName,
                            NomDeFamille = person.LastName,
                        }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<CreateUserModel>();
            }
        }

        public List<CreateUserModel> GetUsers()
        {
            throw new NotImplementedException();
        }

        public int CountUsers(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = string.Empty;
            return _userDataProvider.CountUsers(keyword);
        }

        public bool DeleteUser(string userId)
        {
            return _userDataProvider.DeleteUser(userId);
        }

        public string? GetUserRole(string userId) => _roleDataProvider.GetUserRoleByUserId(userId)?.RoleId;

    }
}
