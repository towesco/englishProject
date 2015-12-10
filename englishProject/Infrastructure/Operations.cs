using englishProject.Infrastructure.Users;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace englishProject.Infrastructure
{
    public class Operations
    {
        private static readonly EnglishProjectDBEntities entities;

        public static UserAppManager usermanager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<UserAppManager>(); }
        }

        public static RoleAppManager rolemanager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<RoleAppManager>();
            }
        }

        public static IAuthenticationManager Authen
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        static Operations()
        {
            entities = new EnglishProjectDBEntities();
        }

        public static List<Box> GetBoxs(Kind kind)
        {
            int _kind = (int)kind;

            return entities.Box.Include("Level").Where(a => a.kind == _kind).OrderBy(a => a.boxNumber).ToList();
        }

        public static UserApp getProfil()
        {
            ClaimsIdentity identity = Authen.User.Identity as ClaimsIdentity;

            string userId =
                identity.Claims.First(a => a.Issuer == "LOCAL AUTHORITY" && a.Type == ClaimTypes.NameIdentifier)
                    .Value.ToString();

            return usermanager.FindById(userId);
        }
    }
}