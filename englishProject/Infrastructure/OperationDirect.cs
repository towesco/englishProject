using englishProject.Infrastructure.HelperClass;
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
    }
}