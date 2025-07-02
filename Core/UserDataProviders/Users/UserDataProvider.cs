using Core.Entity.MyCore;
using Core.ServiceEncryptor;
using Core.ViewModel.Login;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

using Core.Database;
using Core.UserModels;
using Microsoft.EntityFrameworkCore.Storage;
using SAIM.Core.Utilities;

namespace Core.UserDataProviders.Users
{
    public class UserDataProvider : IUserDataProvider
    {
        private readonly MyCoreContext _context;

        public UserDataProvider(MyCoreContext context)
        {
            _context = context;
        }
        public User AuthenticateUser(LoginModel model)
        {
            model.Password = Encryptor.HashPassword(model.Password);
            model.UserName = model.UserName.ToLower();
            return _context.Users.FirstOrDefault(u =>
                (u.Mail.ToLower().Equals(model.UserName)
                 || u.UserName.ToLower().Equals(model.UserName)) &&
                u.HashCode.Equals(model.Password) && u.Status==1);
        }

        public int CountUsers(string keyword)
        {
            keyword = keyword.ToLower();
            return _context.Person.Count(x => (x.FirstName.ToLower().Contains(keyword)
                || x.LastName.ToLower().Contains(keyword)) && x.Status.Equals(1));
        }

        public bool DeleteUser(string userId)
        {
            IDbContextTransaction transaction = null;
            try
            {
                transaction = _context.Database.BeginTransaction();
                var user = _context.Users.Find(userId);
                user.Status = 0;
                _context.SaveChanges();

                var userrole = _context.UserRole.FirstOrDefault(x => x.UserId.Equals(user.Id));
                userrole.Status = 0;
                _context.SaveChanges();

                var person = _context.Person.FirstOrDefault(x => x.Id.Equals(user.PersonId));
                person.Status = 0;
                _context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction?.Rollback();
                return false;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public Person GetPersonById(string personId)
        {
            try
            {
                return _context.Person.Find(personId);
            }
            catch
            {
                return null;
            }
        }

        public List<User> GetREDERUsers()
        {
            throw new NotImplementedException();
        }

        public User GetUserById(string userId)
        {
            return _context.Users.Find(userId);
        }

        public User GetUserByPasswordRequest(string request)
        {
            throw new NotImplementedException();
        }

        public User GetUserByPersonId(string personId)
        {
            return _context.Users.FirstOrDefault(x => x.PersonId.Equals(personId) && x.Status.Equals(1));
        }

        public User GetUserInfo(ResetPasswordModel model)
        {
            throw new NotImplementedException();
        }

        public List<Person> GetUsers(string keyword, int page, int pageSize)
        {
            keyword = keyword.ToLower();
            int limit = pageSize, offset = (page - 1) * pageSize;
            return _context.Person.Where(x => (x.FirstName.ToLower().Contains(keyword)
                                            || x.LastName.ToLower().Contains(keyword)) && x.Status.Equals(1))
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public List<Person> GetUsers()
        {
            throw new NotImplementedException();
        }

        public UserStatus GetUserStatusByLibelle(string libelle)
        {
            throw new NotImplementedException();
        }

        public List<User> GetWmaUsers(string memberAndStatus)
        {
            throw new NotImplementedException();
        }

        public bool IsMailExist(string mail)
        {
            throw new NotImplementedException();
        }

        public bool IsUserNameExist(string username)
        {
            throw new NotImplementedException();
        }

        public string SaveUser(CreateUserModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var userStatus = _context.UserStatus.First(u => u.Name.Equals("ACTIVE"));

                var person = new Person
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.Prenom ?? "",
                    LastName = model.NomDeFamille ?? "",
                    Status = 1
                };

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    PersonId = person.Id,
                    UserName = model.Identifiant,
                    HashCode = Encryptor.HashPassword(model.Password ?? ""),
                    CreatedOn = DateTime.Now,
                    UserStatusId = userStatus.Id,
                    Status = 1,
                };

                var userRole = new UserRole
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    RoleId = model.Role,
                    Status = 1
                };

                _context.Person.Add(person);
                _context.Users.Add(user);
                _context.UserRole.Add(userRole);
                _context.SaveChanges();

                transaction.Commit();

                return user.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool UpdateUser(CreateUserModel model)
        {
            IDbContextTransaction transaction = null;
            try
            {
                transaction = _context.Database.BeginTransaction();
                var user = _context.Users.Find(model.Id);
                user.UserName = model.Identifiant;
                if (model.UpdatePassword) user.HashCode = Encryptor.HashPassword(model.Password ?? "");
                user.Status = 1;
                _context.SaveChanges();
                var person = _context.Person.Find(user.PersonId);
                person.FirstName = model.Prenom ?? "";
                person.LastName = model.NomDeFamille ?? "";
                person.Status = 1;
                _context.SaveChanges();

                var userRole = _context.UserRole.FirstOrDefault(x => x.UserId != null && x.UserId.Equals(user.Id));
                if (userRole == null)
                {
                    var newUserRole = new UserRole
                    {
                        Status = 1,
                        RoleId = model.Role,
                        UserId = model.Id
                    };
                    _context.UserRole.Add(newUserRole);
                    _context.SaveChanges();
                }
                else
                {
                    userRole.RoleId = model.Role;
                    userRole.Status = 1;
                    _context.SaveChanges();
                }
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction?.Rollback();
                return false;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public bool UpdateUserInfo(ConfirmResetPasswordModel model)
        {
            throw new NotImplementedException();
        }
    }
}
