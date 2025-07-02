using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Services.Filters
{
    public class AuthenticationFilter : Attribute, IAuthorizationFilter
    {
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
        }
    }
}