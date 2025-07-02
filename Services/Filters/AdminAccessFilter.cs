using System;
using Core.UserModels;
using Core.UserServices.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Services.Filters;

public class AdminAccessFilter : Attribute, IAuthorizationFilter
{
    private readonly IRoleService _roleService;

    public AdminAccessFilter(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = $"{context.HttpContext.Request.Path.Value}{context.HttpContext.Request.QueryString}";
        var session = context.HttpContext.Session.GetString("user");
        if (session == null)
        {
            context.Result = new RedirectToRouteResult
            (
                new RouteValueDictionary(new
                {
                    action = "login",
                    controller = "user",
                    mustlogin = (!request.Equals("/")).ToString().ToLower(),
                    next = request
                })
            );
        }
        else
        {
            var user = JsonConvert.DeserializeObject<UserModel>(session);
            if (!_roleService.IsAdmin(user.UserRoleId))
            {
                context.Result = new RedirectToRouteResult
                (
                    new RouteValueDictionary(new
                    {
                        action = "login",
                        controller = "user",
                        next = request,
                        accesDenied = "Acces denied!"
                    })
                );
            }
        }
    }
}