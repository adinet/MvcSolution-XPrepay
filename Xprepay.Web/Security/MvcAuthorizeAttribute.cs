﻿using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xprepay.Web.Security
{
    public class MvcAuthorizeAttribute : AuthorizeAttribute
    {
        public MvcAuthorizeAttribute()
        {
            
        }

        public MvcAuthorizeAttribute(string roles)
        {
            this.Roles = roles;
        }
        public MvcAuthorizeAttribute(int type)
        {
          
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionAttrs = filterContext.ActionDescriptor.GetCustomAttributes(true);

            if (actionAttrs.Any(x => x is AllowAnonymousAttribute))
            {
                return;
            }
            var actionAttr = actionAttrs.FirstOrDefault(x => x is MvcAuthorizeAttribute);
            if (actionAttr != null)
            {
                ((MvcAuthorizeAttribute)actionAttr).Authenticate(filterContext);
                return;
            }
            var controllerAttrs = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true);
            if (controllerAttrs.Any(x => x is AllowAnonymousAttribute))
            {
                return;
            }
            var controllerAttr = controllerAttrs.FirstOrDefault(x => x is MvcAuthorizeAttribute);
            if (controllerAttr != null)
            {
                ((MvcAuthorizeAttribute)controllerAttr).Authenticate(filterContext);
                return;
            }
            this.Authenticate(filterContext);
        }

        public void Authenticate(AuthorizationContext filterContext)
        {
            var context = filterContext.RequestContext.HttpContext;
            var isAuthenticated = context.Request.IsAuthenticated;
            if (!string.IsNullOrEmpty(this.Roles))
            {
                isAuthenticated = isAuthenticated && this.Roles.Split(',').Any(r => context.User.IsInRole(r));
            }
            if (isAuthenticated)
            {
                return;
            }

            if (HttpContext.Current.Request["ajax"] == "true")
            {
                filterContext.Result = new StandardJsonResult()
                {
                    Message = context.Request.IsAuthenticated ? "Please login" : "You don't have sufficient permission"
                };
            }
            else
            {
                if (HttpContext.Current.Request.RawUrl.ToLower().Contains("mobile"))
                {
                    filterContext.Result = new RedirectResult("public/home/mobilelogin?returnUrl=" + HttpContext.Current.Request.RawUrl);
                }
                else
                {
                    filterContext.Result = new RedirectResult("/login?returnUrl=" + HttpContext.Current.Request.RawUrl);
                }
            }
        }
    }
}
