using AutoMapper.Internal;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Ninject.Infrastructure.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public static class OperationsAdmin
    {
        private static readonly wordboxe_englishEntities entities = new wordboxe_englishEntities();

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

        public static List<log> GetLogs()
        {
            return entities.log.OrderByDescending(a => a.Date).Take(30).ToList();
        }

        public static List<boxLevelCount> getLevelCount()
        {
            List<boxLevelCount> boxLevelCounts = new List<boxLevelCount>();

            foreach (Box box in entities.Box.ToList())
            {
                boxLevelCount boxLevelCount = new boxLevelCount();

                if (box.boxNumber == 1)
                {
                    boxLevelCount.BoxName = string.Format("({0})  {1} toplam kelime", box.boxName, box.Level.Sum(a => a.Word.Count()));
                    boxLevelCount.Levels = box.Level.ToDictionary(level => level.levelId,
                        level => string.Format("{0}.seviye  <strong>{1}</strong> kelime", level.levelNumber, level.Word.Count()));
                }
                else if (box.boxNumber == 2)
                {
                    boxLevelCount.BoxName = string.Format("({0})  {1} toplam kelime", box.boxName, box.Level.Sum(a => a.SynonymWord.Count()));
                    boxLevelCount.Levels = box.Level.ToDictionary(level => level.levelId,
                        level => string.Format("{0}.seviye  <strong>{1}</strong> kelime", level.levelNumber, level.SynonymWord.Count()));
                }
                else if (box.boxNumber == 3)
                {
                    boxLevelCount.BoxName = string.Format("({0})  {1} toplam kelime", box.boxName, box.Level.Sum(a => a.SynonymWord.Count()));
                    boxLevelCount.Levels = box.Level.ToDictionary(level => level.levelId,
               level => string.Format("{0}.seviye  <strong>{1}</strong> kelime", level.levelNumber, level.SynonymWord.Count()));
                }

                boxLevelCounts.Add(boxLevelCount);
            }
            return boxLevelCounts;
        }

        public static UserProfilView GetUserProfilViewMenu(string GetUserId)
        {
            UserProfilView userProfilView = new UserProfilView();

            UserApp user = usermanager.FindById(GetUserId);
            userProfilView.picture = user.PicturePath;
            userProfilView.userName = user.UserName;

            int total;
            try
            {
                total = entities.levelUserProgress.Where(a => a.userId == GetUserId).Sum(a => a.puan);
            }
            catch (Exception)
            {
                total = 0;
            }
            userProfilView.TotalPuan = total;

            userProfilView.UserProfilBoxs = entities.userProggress(GetUserId).ToList();

            if (!userProfilView.UserProfilBoxs.Any())
            {
                Box b = entities.Box.First();
                userProfilView.UserProfilBoxs.Add(new userProggress_Result()
                {
                    boxId = b.boxId,
                    boxName = b.boxName,
                    CurrentLevel = 1,
                    Progress = 0,
                    CurrentProgress = 0
                });
            }

            return userProfilView;
        }

        public static List<UserProfilView> GetUsers()
        {
            return usermanager.Users.ToEnumerable().OrderByDescending(a => a.createTime).Select(a => GetUserProfilViewMenu(a.Id)).ToList();
        }
    }
}