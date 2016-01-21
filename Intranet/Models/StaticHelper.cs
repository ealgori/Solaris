using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace Intranet.Models
{
    public static class StaticHelper
    {
        public static bool IsAdmin(this IPrincipal User)
        {
            if (User.Identity.Name == @"ERICSSIN\ealgori" || User.Identity.Name == @"ERICSSIN\esovalr" || User.Identity.Name == @"ERICSSIN\echeale")
            {
                return true;
            }
            return false;
        }
       
    }
}