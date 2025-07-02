

using Core.Entity.MyCore;
using Core.ServiceEncryptor;
using Core.UserModels;
using Core.UserServices.Role;
using Core.UserServices.Users;
using Core.ViewModel.Login;
using EDMS2025.Models.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Services.Filters;
using System.Web;
using System.Xml.Linq;

namespace EDMS2025.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [Route("Index/{step}")]
        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult Index(string? deleted, string? keyword, string? page)
        {
            ViewBag.step = 23;
            if (!int.TryParse(page, out var pageNumber) || pageNumber < 1) pageNumber = 1;
            var pageSize = 5;
            ViewBag.users = _userService.GetUsers(keyword, pageNumber, pageSize);
            var resultCount = _userService.CountUsers(keyword);
            ViewBag.Pagination = PageUtility.MakePagination(pageSize, resultCount, pageNumber, $"{HttpUtility.UrlEncode(Encryption.Encrypt(23.ToString()))}");
            ViewBag.keyword = keyword;
            if (!string.IsNullOrEmpty(deleted)) ViewBag.success = true;
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? invalidLogin, string? mustlogin, string? accesDenied, string next, string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            if (!string.IsNullOrEmpty(mustlogin) && mustlogin.Equals("true")) ViewBag.Error = "You must login first!";
            if (!string.IsNullOrEmpty(accesDenied)) ViewBag.Error = "Acces denied!";
            if (!string.IsNullOrEmpty(invalidLogin) && invalidLogin.Equals("true")) ViewBag.Error = "Invalid username or password";
            if (!string.IsNullOrEmpty(next)) ViewBag.Next = next;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    var userModel = _userService.AuthenticateUser(model);
                    if (userModel == null) {
                        return Redirect(string.IsNullOrEmpty(model.Next)
                        ? "/user/login?invalidLogin=true"
                        : $"/user/login?invalidLogin=true&next={model.Next}");
                    }
                    else
                    {
                        //set Session   http://localhost:5191/EDMS/DocumentReviewModificationSearch/p%2fokpyVtlz62AlsPh3PdSg
                        var session = JsonConvert.SerializeObject(userModel);
                        HttpContext.Session.SetString("user", session);
                        var elements = _roleService.GetElementsByRoleId(userModel.UserRoleId);
                        var url = elements[0].ChildElements.Any()
                                        ? elements[0].ChildElements[0].Url
                                        : elements[0].Url;
                        var segments = url.TrimStart('/').Split('/');
                        var (controller, action) = ("", "");
                        if (segments.Length >= 2)
                        {
                            (controller, action) = (segments[0], segments[1]); // [Controller, Action]
                        }
                        else
                        {
                            (controller, action) = ("", segments[0]);
                        }

                        var action__ = elements[0].ChildElements.Any() 
                                        ? action +'/'+ HttpUtility.UrlEncode(Encryption.Encrypt(elements[0].ChildElements[0].Step.ToString())) 
                                        : action + '/' + HttpUtility.UrlEncode(Encryption.Encrypt(elements[0].Step.ToString()));

                        var view = string.IsNullOrEmpty(model.Next) 
                                    ? $"{action__}"
                                    : $"{model.Next}";
                        return Redirect(view);
                    }
                }
                return View(model);
            }
            catch (Exception ex) {
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            var session = HttpContext.Session.GetString("user");
            var connectedUser = !string.IsNullOrEmpty(session) ? JsonConvert.DeserializeObject<UserModel>(session) : null;
            if (connectedUser != null)
            {
                _userService.CloseSession(connectedUser);
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }

        [Route("CreateUser/{step}")]
        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult CreateUser(string? email, string? error, string? success)
        {
            ViewBag.step = 21;
            if (!string.IsNullOrEmpty(email)) ViewBag.error = "L'identifiant existe déjà pour un autre compte!";
            if (!string.IsNullOrEmpty(error)) ViewBag.error = "Une erreur s'est produite, veuillez réessayer!";
            if (!string.IsNullOrEmpty(success) && success.Equals("saved")) ViewBag.success = "Utilisateur créé avec succès!";
            ViewBag.Roles = _roleService.GetAllRoles();

            //return View("~/Views/Base/User/CreateUser.cshtml");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUser(CreateUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = _roleService.GetAllRoles();

                    return View("~/Views/User/CreateUser.cshtml", model);
                }
                var isUserSaved = string.IsNullOrEmpty(model.Id) ? _userService.SaveUser(model) : _userService.UpdateUser(model);
                if (isUserSaved) return RedirectToAction("Index", new { step = HttpUtility.UrlEncode(Encryption.Encrypt(23.ToString())), keyword = model.NomDeFamille, saved = true });
                //return Redirect($"/user/createuser?keyword={model.NomDeFamille}&saved=true");
                ViewBag.error = "Une erreur s'est produite, veuillez réessayer!";
                return View("~/Views/User/CreateUser.cshtml", model);
            }
            catch (Exception e)
            {
                return Redirect($"/user/createuser?error=occured&ex={e?.ToString().ToLower()}");
            }
        }

        [Route("Edit/{id}")]
        [TypeFilter(typeof(AuthenticationFilter))]
        public IActionResult Edit(string id)
        {
            ViewBag.step = 21;
            var model = _userService.GetUserById(id);
            ViewBag.Roles = _roleService.GetAllRoles();
            return View("~/Views/User/CreateUser.cshtml", model);
        }

        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var areDeleted = _userService.DeleteUser(userId);
            var Url = $"Index/{HttpUtility.UrlEncode(Encryption.Encrypt(23.ToString()))}";
            return Json(new { status = areDeleted ? 1 : 0, url = Url });
        }
    }
}
