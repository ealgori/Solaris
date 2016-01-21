using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.Models;

namespace Intranet.ActionFilters
{
   
        [AttributeUsage(AttributeTargets.All)]
        public class OnlyAdmin : ActionFilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                if (!filterContext.HttpContext.User.IsAdmin())
                {
                    ViewResult result = new ViewResult();
                    result.ViewName = "Error";
                    result.ViewBag.ErrorMessage = "Only admins are allowed";
                    filterContext.Result = result;
                }
            }
        }

    
}