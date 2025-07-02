using System;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Entity.MyCore;
using Core.UserModels;
using Core.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.UserServices.Role
{
    public class RoleService : IRoleService
    {
        private readonly MyCoreContext _context;
        //private readonly IApplicationDataProvider _applicationDataProvider;

        public RoleService(MyCoreContext context)
        {
            _context = context;
            //_applicationDataProvider = applicationDataProvider;
        }

        public bool CanUserAccessLink(string userId, string url)
        {
            try
            {
                var userRole = _context.UserRole.First(x => x.UserId.Equals(userId));
                var roleAccess = _context.RoleAccessControl.Where(u => u.RoleId.Equals(userRole.RoleId)).ToList();
                foreach (var role in roleAccess)
                {
                    var element = _context.Element.Find(role.ElementId);
                    if (url.ToLower().Contains(element.Url.ToLower())) return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public bool IsAdmin(string roleId)
        {
            try
            {
                return _context.Role.FirstOrDefault(x => x.Id == roleId)?.Name.ToLower() == "administrator";
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetRoleAccessibleMenu(string roleId)
        {
            try
            {
                return _context.RoleAccessControl.Where(x => x.RoleId.Equals(roleId)).Select(x => x.ElementId).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public List<int> GetAccesBoutton(string MyId)
        {
            int MyIdInt = int.Parse(MyId);
            try
            {
                var allElementBouttons = _context.ElementBoutton.ToList();

                return allElementBouttons
                    .Where(eb => !string.IsNullOrWhiteSpace(eb.ElementId) && // Ensure ElementId is not empty
                        eb.ElementId.Split(',')
                            .Select(id => id.Trim()) // Trim spaces
                            .Where(id => !string.IsNullOrEmpty(id)) // Remove empty values
                            .Select(int.Parse) // Convert valid values to int
                            .Contains(MyIdInt))
                    .Select(eb => eb.Id)
                    .ToList();
            }
            catch
            {
                return new List<int>();
            }
        }

        public bool UpdateRoleAccess(string roleId, IEnumerable<string> menus)
        {
            IDbContextTransaction transaction = null;
            try
            {
                using (transaction = _context.Database.BeginTransaction())
                {
                    var roleAccessibleMenu = _context.RoleAccessControl.Where(x => x.RoleId.Equals(roleId)).ToList();
                    _context.RoleAccessControl.RemoveRange(roleAccessibleMenu);
                    _context.SaveChanges();

                    foreach (var menu in menus)
                    {
                        var roleAccessControl = new RoleAccessControl
                        {
                            Id = Guid.NewGuid().ToString().ToUpper(),
                            RoleId = roleId,
                            ElementId = menu,
                            Status = 1
                        };
                        _context.RoleAccessControl.Add(roleAccessControl);
                        _context.SaveChanges();
                    }
                    transaction.Commit();
                    return true;
                }
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

        public bool UpdateAccessButton(string MyId, IEnumerable<string> selectedBtns, IEnumerable<string> notSelectedBtns) {
            IDbContextTransaction transaction = null;
            try
            {
                using (transaction = _context.Database.BeginTransaction())
                {
                    foreach (var button in notSelectedBtns)
                    {
                        int buttonId = int.Parse(button);
                        int intMyId = int.Parse(MyId);
                        var elButton = _context.ElementBoutton.FirstOrDefault(eb => eb.Id == buttonId);
                        if (elButton != null)
                        {
                            var elementIds = !string.IsNullOrWhiteSpace(elButton.ElementId)
                                    ? elButton.ElementId.Split(',')
                                        .Select(id => id.Trim())
                                        .Where(id => !string.IsNullOrEmpty(id))
                                        .Select(int.Parse)
                                        .ToList()
                                    : new List<int>();
                            if (elementIds.Contains(intMyId))
                            {
                                elementIds.Remove(intMyId);
                                elButton.ElementId = string.Join(",", elementIds);
                                _context.Attach(elButton);
                                _context.Entry(elButton).Property(e => e.ElementId).IsModified = true;
                                _context.SaveChanges();
                            }
                        }
                    }

                    foreach (var button in selectedBtns)
                    {
                        int buttonId = int.Parse(button);
                        int intMyId = int.Parse(MyId);
                        var elButton = _context.ElementBoutton.FirstOrDefault(eb => eb.Id == buttonId);
                        if (elButton != null)
                        {
                            var elementIds = !string.IsNullOrWhiteSpace(elButton.ElementId)
                                    ? elButton.ElementId.Split(',')
                                        .Select(id => id.Trim())
                                        .Where(id => !string.IsNullOrEmpty(id))
                                        .Select(int.Parse)
                                        .ToList()
                                    : new List<int>();
                            if (!elementIds.Contains(intMyId))
                            {
                                elementIds.Add(intMyId);
                                elButton.ElementId = string.Join(",", elementIds);
                                _context.Attach(elButton);
                                _context.Entry(elButton).Property(e => e.ElementId).IsModified = true;
                                _context.SaveChanges();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
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

        public List<ElementModel> GetAllElements()
        {
            try
            {
                var result = new List<ElementModel>();
                var childrenTemps = _context.ElementHierarchy.Select(x => x.ElementId).Distinct().ToList();
                var parentsId = _context.Element.Where(x => !childrenTemps.Contains(x.Id)).Select(x => x.Id).Distinct()
                    .ToList();
                foreach (var parentId in parentsId)
                {
                    var parentElement = _context.Element.First(x => x.Id.Equals(parentId));
                    var parentElementModel = new ElementModel
                    {
                        Id = parentElement.Id,
                        Description = parentElement.Description,
                        Url = parentElement.Url,
                        Name = parentElement.Name,
                        MyId = parentElement.MyId
                    };
                    var childsId = _context.ElementHierarchy.Where(x => x.ParentElementId.Equals(parentId))
                        .Select(x => x.ElementId).ToList();
                    foreach (var childId in childsId)
                    {
                        parentElementModel.ChildElements.Add(_context.Element.Where(x => x.Id.Equals(childId))
                            .Select(element => new ElementModel
                            {
                                Id = element.Id,
                                Description = element.Description,
                                Url = element.Url,
                                Name = element.Name,
                                MyId = element.MyId

                            }).First());
                    }

                    result.Add(parentElementModel);
                }

                return result;
            }
            catch
            {
                return new List<ElementModel>();
            }
        }

        public List<ElementBoutton> GetElementBouttons()
        {
            try
            {
                var elemBoutton = new List<ElementBoutton>();                
                elemBoutton = _context.ElementBoutton.ToList();
                return elemBoutton;
            }
            catch {
                return new List<ElementBoutton>();
            }
        }

        public List<RoleModel> GetAllRoles()
        {
            try
            {
                return _context.Role.Select(role => new RoleModel { Id = role.Id, Name = role.Name, Description = role.Description }).OrderBy(x => x.Name).ToList();
            }
            catch
            {
                return new List<RoleModel>();
            }
        }

        public bool AsignRoleToUsers(ICollection<UserRoleModel> models)
        {
            IDbContextTransaction transaction = null;
            try
            {
                using (transaction = _context.Database.BeginTransaction())
                {
                    foreach (var model in models)
                    {
                        var userRole = new UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = model.UserId,
                            RoleId = model.RoleId,
                            Status = 1
                        };
                        _context.UserRole.Add(userRole);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception e)
            {
                transaction?.Rollback();
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public bool UpdateUserRole(UserRoleModel model)
        {
            try
            {
                var userRole = _context.UserRole.First(u => u.UserId.Equals(model.UserId));
                userRole.RoleId = model.RoleId;
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool AsignRoleToUser(UserRoleModel model)
        {
            try
            {
                UserRole existingUserRole = null;
                try
                {
                    existingUserRole = _context.UserRole.First(x => x.UserId.Equals(model.UserId));
                    existingUserRole.RoleId = model.RoleId;
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    // ignored
                }

                if (existingUserRole == null)
                {
                    var userRole = new UserRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = model.UserId,
                        RoleId = model.RoleId,
                        Status = 1
                    };
                    _context.UserRole.Add(userRole);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public bool AddRole(RoleModel model)
        {
            try
            {
                var role = new Entity.MyCore.Role
                {
                    Id = Guid.NewGuid().ToString(),
                    LastUpdatedBy = model.LastUpdatedBy,
                    Description = model.Description,
                    Name = model.Name,
                    Status = 1
                };
                _context.Role.Add(role);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateRole(RoleModel model)
        {
            IDbContextTransaction transaction = null;
            try
            {
                using (transaction = _context.Database.BeginTransaction())
                {
                    var role = _context.Role.Find(model.Id);
                    if (role == null)
                    {
                        return false;
                    }
                    else { 
                        role.Description = model?.Description;
                        role.Name = model?.Name;
                        _context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch
            {
                transaction?.Rollback();
                return false;
            }
            finally { transaction?.Dispose(); }
        }

        public bool DeleteRole(string id)
        {
            try
            {
                var role = _context.Role.Find(id);
                _context.Remove(role);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<ElementModel> GetElementsByRoleId(string roleId)
        {
            try
            {
                var result = new List<ElementModel>();

                var accessibleElements = _context.RoleAccessControl
                    .Where(x => x.RoleId.Equals(roleId))
                    .Select(x => x.ElementId).Distinct().ToList();

                foreach (var element in accessibleElements)
                {
                    //var elementData = _context.Element.First(x => x.Id.Equals(element));
                    var elementData = _context.Element.Where(x => x.Id.Equals(element)).First();
                    var elementModel = new ElementModel
                    {
                        Id = elementData.Id,
                        MyId = elementData.MyId,
                        Description = elementData.Description,
                        Url = $"{elementData.Url}".Replace(@"//", @"/"),
                        Name = elementData.Name,
                        Order = elementData.Order,
                        Step = elementData.Step,
                    };
                    var parentElementHierarchy = _context.ElementHierarchy.FirstOrDefault(x => x.ElementId.Equals(element));
                    if (parentElementHierarchy != null)
                    {
                        var parent = _context.Element.Find(parentElementHierarchy.ParentElementId);
                        var parentElementInResult = result.FirstOrDefault(x => x.Id.Equals(parent?.Id));
                        if (parentElementInResult != null)
                        {
                            parentElementInResult.ChildElements.Add(elementModel);
                        }
                        else
                        {
                            var parentElementModel = new ElementModel
                            {
                                Id = parent.Id,
                                MyId = parent.MyId,
                                Description = parent.Description,
                                Url = parent.Url,
                                Name = parent.Name,
                                Order = elementData.Order,
                                Step = elementData.Step,
                            };

                            parentElementModel.ChildElements.Add(elementModel);
                            result.Add(parentElementModel);
                        }
                    }
                    else
                    {
                        result.Add(elementModel);
                    }
                }
                //return result.ToList();
                return result.OrderBy(x => x.Order).ToList();
            }
            catch
            {
                return new List<ElementModel>();
            }
        }

        public List<ElementBoutton> GetElementsBouttonByStep(int step)
        {
            try
            {
                var elemBouttons = _context.ElementBoutton
                    .Where(eb => !string.IsNullOrEmpty(eb.ElementId))
                    .Include(eb => eb.Element)
                    .ToList();
                var filterdBoutton = elemBouttons
                    .Where(eb =>
                        eb.ElementId.Split(',')
                            .Select(MyId => long.Parse(MyId.Trim()))
                            .Any(id => _context.Element.Any(e => e.MyId == id && e.Step == step)))
                    .ToList();

                return filterdBoutton;
            }
            catch
            {
                return new List<ElementBoutton>();
            }
        }

    }
}