using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParentsRules.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
        //This will provide a link for a user to associate their account with the requester.
        public static string AssociatedUserLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            string returnUrl = "/Account/Login";
            return urlHelper.Action(
                action: nameof(AccountController.Register),
                controller: "Account",
                values: new { userId, code, returnUrl },
                protocol: scheme);
        }
    }
}
