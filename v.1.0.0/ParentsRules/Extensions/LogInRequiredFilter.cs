using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Extensions
{
    public class LogInRequiredFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!AttributeManager.HasAttribute(context, typeof(LogInRequired))) return;

            if (context.HttpContext.User.Identity.IsAuthenticated) return;

            context.Result = new RedirectResult("/login?ReturnUrl=" + Uri.EscapeDataString(context.HttpContext.Request.Path));
        }
    }
    public class LogInRequired : Attribute
    {
        public LogInRequired()
        {

        }
    }
    public class AttributeManager
    {
        public static Boolean HasAttribute(AuthorizationFilterContext context, Type targetAttribute)
        {
            var hasAttribute = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                hasAttribute = controllerActionDescriptor
                                                .MethodInfo
                                                .GetCustomAttributes(targetAttribute, false).Any();
            }

            return hasAttribute;
        }
    }
}
