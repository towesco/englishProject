using englishProject.Models;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace englishProject.Infrastructure
{
    public static class OperationDirect
    {
        private static EnglishProjectDBEntities entities;

        static OperationDirect()
        {
            entities = new EnglishProjectDBEntities();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="score">kullanıcının günlük hedefini güncelle</param>
        /// <param name="update">Var olan günlük hedefin güncellenmesini sağllar </param>
        ///     /// <param name="UserId">Var olan günlük hedefin güncellenmesini sağllar </param>
        public static bool UpdateTargetDailyTargetScore(int score, string UserId, bool update = true)
        {
            UserDetail userDetail = entities.UserDetail.FirstOrDefault(a => a.userId == UserId);

            if (userDetail != null)
            {
                if (update)
                {
                    userDetail.DailyTargetScore = score;
                    entities.SaveChanges();
                }
            }
            else
            {
                UserDetail u = new UserDetail() { DailyTargetScore = score, userId = UserId };
                entities.UserDetail.Add(u);
                entities.SaveChanges();
            }
            return true;
        }
    }
}