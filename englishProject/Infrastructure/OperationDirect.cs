using englishProject.Infrastructure;
using englishProject.Infrastructure.HelperClass;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI.WebControls;

namespace englishProject.Infrastructure
{
    public static class OperationDirect
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        private static wordboxe_englishEntities entities;

        static OperationDirect()
        {
            entities = new wordboxe_englishEntities();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="score">kullanıcının günlük hedefini güncelle</param>
        /// <param name="update">Var olan günlük hedefin güncellenmesini sağllar </param>
        ///     /// <param name="UserId">Var olan günlük hedefin güncellenmesini sağllar </param>
        public static bool UpdateTargetDailyTargetScore(int score, string UserId, bool update = true)
        {
            try
            {
                UserDetail userDetail = entities.UserDetail.FirstOrDefault(a => a.userId == UserId);

                if (userDetail != null)
                {
                    if (update)
                    {
                        userDetail.DailyTargetScore = score;
                        userDetail.SoundEffect = true;
                        entities.SaveChanges();
                    }
                }
                else
                {
                    UserDetail u = new UserDetail() { DailyTargetScore = score, userId = UserId, SoundEffect = true };
                    entities.UserDetail.Add(u);
                    entities.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateTargetDailyTargetScore", ex);
                return false;
            }
        }

        public static bool UpdateUserDetail(UserDetail userDetail)
        {
            try
            {
                UserDetail user = entities.UserDetail.Find(userDetail.userId);
                user.SoundEffect = userDetail.SoundEffect;
                entities.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateUserDetail", ex);
                return false;
            }
        }

        public static string GetCityName(int cityId)
        {
            return entities.City.Find(cityId).Name;
        }

        public static List<ScoreTableBox> GetScoreTable()
        {
            List<ScoreTableBox> scoreTableBoxs = new List<ScoreTableBox>();
            foreach (Box box in entities.Box.ToList())
            {
                ScoreTableBox _scoreTableBox = new ScoreTableBox { BoxName = box.boxName };
                List<ScoreTableLevel> scoreTableLevels = new List<ScoreTableLevel>();

                foreach (var level in box.Level.ToList())
                {
                    ScoreTableLevel _scoreTableLevel = new ScoreTableLevel();
                    int levelTotal = 0;

                    _scoreTableLevel.LevelNumber = level.levelNumber;
                    _scoreTableLevel.LevelName = level.levelName;

                    for (int i = 1; i <= level.levelSubLevel; i++)
                    {
                        levelTotal += level.Word.ToList().Sum(word => i * level.levelPuan);
                    }
                    _scoreTableLevel.Score = levelTotal;
                    _scoreTableBox.boxTotalScore += levelTotal;

                    scoreTableLevels.Add(_scoreTableLevel);
                }
                _scoreTableBox.ScoreTableLevels = scoreTableLevels;
                scoreTableBoxs.Add(_scoreTableBox);
            }

            return scoreTableBoxs;
        }

        public static bool CommentIssueSave(CommentIssueVM viewmodel)
        {
            try
            {
                CommentIssue _commentIssue = (CommentIssue)viewmodel.Kind;

                switch (_commentIssue)
                {
                    case CommentIssue.comment:

                        comment c = new comment
                        {
                            userId = viewmodel.UserId,
                            commentNote = viewmodel.CommentIssue,
                            commentKind = 1,
                            commentDate = DateTime.Now,
                            commentExceptId = viewmodel.ExceptId,
                        };
                        if (viewmodel.ReplyId != null)
                        {
                            c.commentReplyId = viewmodel.ReplyId;
                        }

                        entities.comment.Add(c);
                        entities.SaveChanges();

                        break;

                    case CommentIssue.issue:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("CommentIssueSave", ex);
                return false;
            }
        }

        public static bool GetCommentAny(int levelId)
        {
            return entities.comment.Any(a => a.commentKind == 1 && a.commentExceptId == levelId);
        }

        public static UserApp GetUser(string userId)
        {
            return usermanager.FindById(userId);
        }

        public static void PageError(string url)
        {
            logger.Info(url);
        }
    }
}