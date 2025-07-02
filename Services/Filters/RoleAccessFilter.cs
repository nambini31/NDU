using System;
using Core.UserModels;
using Core.UserServices.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Services.Filters
{
    public class RoleAccessFilter : Attribute, IAuthorizationFilter
    {
        private readonly IRoleService _roleService;

        public RoleAccessFilter(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = $"{context.HttpContext.Request.Path.Value}{context.HttpContext.Request.QueryString}";
            var session = context.HttpContext.Session.GetString("user");
            //var enseigneSession = context.HttpContext.Session.GetString("enseigne");
            if (session == null)
            {
                context.Result = new RedirectToRouteResult
                (
                    new RouteValueDictionary(new
                    {
                        action = "index",
                        controller = "account",
                        mustlogin = (!request.Equals("/")).ToString().ToLower(),
                        next = request
                    })
                );
            }
            //else
            //{
            //    var user = JsonConvert.DeserializeObject<UserModel>(session);
            //    var enseigne = JsonConvert.DeserializeObject<ApplicationModel>(enseigneSession);
            //    if (!_roleService.CanUserAccessLink(user.Id, enseigne.Id, request.Replace("/" + enseigne.Nom, "")))
            //    {
            //        context.Result = new RedirectToRouteResult
            //        (
            //            new RouteValueDictionary(new
            //            {
            //                action = "accessdenied",
            //                controller = "error",
            //                req = context.HttpContext.Request.Path.Value
            //            })
            //        );
            //    }
            //}
        }
    }
}