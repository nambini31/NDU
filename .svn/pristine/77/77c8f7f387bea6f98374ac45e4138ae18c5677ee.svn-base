using System.Linq;
using Core.ServiceEncryptor;
using System.Web;
using Core.UserModels;
using Core.UserServices.Role;
using Core.UserServices.Users;
using EDMS2025.Models.Utility;
using Microsoft.AspNetCore.Mvc;
using Services.Filters;

namespace EDMS2025.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class RoleManagementController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RoleManagementController(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }

        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult Index()
        {
            //return View("~/Views/Base/roleManagement/AssignRole.cshtml");      
            return View("AssignRole");
        }

        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult AssignRole(string? keyword, string? page)
        {
            if (!int.TryParse(page, out var pageNumber) || pageNumber <= 0) pageNumber = 1;
            var pageSize = 5;
            var assignRoleModel = new AssignRoleModel
            {
                Users = _userService.GetUsers(keyword ?? "", pageNumber, pageSize),
                Roles = _roleService.GetAllRoles(),
            };
            var resulCount = _userService.CountUsers(keyword ?? "");
            ViewBag.Keyword = keyword;
            ViewBag.Pagination = PageUtility.MakePagination(pageSize, resulCount, pageNumber,
                $"/rolemanagement/assignrole?keyword={keyword}");
            return View("AssignRole", assignRoleModel);
        }

        [HttpGet]
        public JsonResult LoadUserRole(string userId)
        {
            var role = _userService.GetUserRole(userId);
            return Json(new { status = !string.IsNullOrEmpty(role) ? 1 : 0, roleId = role });
        }

        [HttpPost]
        public JsonResult SetUserRole(string roleId, string userId)
        {
            var userRoleModel = new UserRoleModel()
            {
                UserId = userId,
                RoleId = roleId
            };
            return Json(_roleService.AsignRoleToUser(userRoleModel) ? new { Status = 1, Message = "OK" } : new { Status = 0, Message = "ERROR" });
        }


        [Route("AddRole/{step}")]
        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult AddRole(string? error, string? success)
        {
            ViewBag.step = 24;
            if (!string.IsNullOrEmpty(error)) ViewBag.error = "An error has occured, please try again!";
            if (!string.IsNullOrEmpty(success)) ViewBag.success = "Role saved successfully!";
            var roleModel = new RoleModel()
            {
                Roles = _roleService.GetAllRoles()
            };
            return View(roleModel);

        }

        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AdminAccessFilter))]
        [HttpPost]
        public IActionResult SaveRole(RoleModel model)
        {
            var isUserSaved = string.IsNullOrEmpty(model.Id) ? _roleService.AddRole(model) : _roleService.UpdateRole(model);
            var encrypteStep = HttpUtility.UrlEncode(Encryption.Encrypt(24.ToString()));
            return Redirect(isUserSaved ? $"/Addrole/{encrypteStep}?success=saved" : $"/Addrole/{encrypteStep}?error=not_saved");
        }

        [HttpPost]
        public JsonResult UpdateRoleAccess(string roleId,string[] menu)
        {
            var isUptated = _roleService.UpdateRoleAccess(roleId, menu);
            return Json(new { status = isUptated ? 1 : 0, message = isUptated ? "OK" : "ERROR", data = "" });
        }

        [HttpPost]
        public JsonResult UpdateAccessButton(string MyId, string[] selectedBtns, string[] notSelectedBnts)
        {
            var isUptated = _roleService.UpdateAccessButton(MyId, selectedBtns, notSelectedBnts);
            return Json(new { status = isUptated ? 1 : 0, message = isUptated ? "OK" : "ERROR", data = "" });
        }

        [Route("RoleAccessControl/{step}")]
        [TypeFilter(typeof(AdminAccessFilter))]
        public IActionResult RoleAccessControl()
        {
            ViewBag.step = 22;
            var model = new RoleAccessControlModel
            {
                Roles = _roleService.GetAllRoles(),
                ParentElements = _roleService.GetAllElements(),
                ElementsBoutton = _roleService.GetElementBouttons(),
            };

            return View("RoleAccessControl", model);
        }

        [HttpGet]
        public JsonResult GetRoleAccessibleMenu(string roleId)
        {
            var accessibleMenu = _roleService.GetRoleAccessibleMenu(roleId);
            return Json(new { data = accessibleMenu });
        }

        [HttpGet]
        public JsonResult GetAccesBouttonByElemntMyId(string MyId)
        {
            var accesBoutton = _roleService.GetAccesBoutton(MyId);
            return Json(new { data = accesBoutton });
        }



    }
}
