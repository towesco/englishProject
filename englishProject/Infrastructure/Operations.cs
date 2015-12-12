using englishProject.Infrastructure.Users;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Results;

namespace englishProject.Infrastructure
{
    public class Operations
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EnglishProjectDBEntities entities;

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

        public static string GetUserId
        {
            get
            {
                ClaimsIdentity identity = Authen.User.Identity as ClaimsIdentity;
                string userId =
             identity.Claims.First(a => a.Issuer == "LOCAL AUTHORITY" && a.Type == ClaimTypes.NameIdentifier)
                 .Value;
                return userId;
            }
        }

        public Operations()
        {
            entities = new EnglishProjectDBEntities();
        }

        public List<Box> GetBoxs(Kind kind)
        {
            int _kind = (int)kind;

            return entities.Box.Include("Level").Where(a => a.kind == _kind).OrderBy(a => a.boxNumber).ToList();
        }

        public UserApp getProfil()
        {
            return usermanager.FindById(GetUserId);
        }

        private LevelExam GetQuestionshuffle(IEnumerable<Word> words, SubLevel subLevel, List<Word> questionsList)
        {
            Random rnd = new Random();
            List<Questions> List = new List<Questions>();
            LevelExam exam = null;
            List<string> siklar = new List<string>();

            Questions q = null;

            switch (subLevel)
            {
                case SubLevel.Temel:

                    foreach (Word item in words)
                    {
                        q = new Questions { Question = item.wordTranslate, QuestionCorrect = item.wordTurkish };

                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word>() { item };
                        //4 tane yanlış evap
                        siklar = questionsList.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTurkish).ToList();
                        //1 tane doğru cevap şıklar ekleniyor
                        siklar.Add(item.wordTurkish);

                        q = new Questions
                        {
                            Question = item.wordTranslate,
                            QuestionCorrect = item.wordTurkish,
                            QestionsOptions = siklar.OrderBy(a => rnd.Next()).ToList()
                        };

                        List.Add(q);
                    }

                    break;

                case SubLevel.İleri:

                    foreach (Word item in words)
                    {
                        q = new Questions { Question = item.wordTurkish, QuestionCorrect = item.wordTranslate };

                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word>() { item };
                        //4 tane yanlış evap
                        siklar = questionsList.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTranslate).ToList();
                        //1 tane doğru cevap şıklar ekleniyor
                        siklar.Add(item.wordTranslate);

                        q = new Questions
                        {
                            Question = item.wordTurkish,
                            QuestionCorrect = item.wordTranslate,
                            QestionsOptions = siklar.OrderBy(a => rnd.Next()).ToList()
                        };

                        List.Add(q);
                    }

                    break;

                case SubLevel.Mükemmel:

                    foreach (Word item in words)
                    {
                        q = new Questions { Question = item.wordTurkish, QuestionCorrect = item.wordTranslate };

                        List.Add(q);
                    }

                    break;
            }

            exam = new LevelExam() { SubLevel = subLevel, Questions = List.OrderBy(a => rnd.Next()).ToList() };

            return exam;
        }

        public Tuple<LevelExam, Level> GetExam(SubLevel subLevel, int levelNumber, int kind)
        {
            Level level = entities.Level.Find(levelNumber, kind);
            levelUserProgress progress =

                entities.levelUserProgress.FirstOrDefault(
                  a => a.userId == GetUserId && a.levelNumber == level.levelNumber && a.kind == level.kind);

            LevelExam exam = GetQuestionshuffle(level.Word.ToList(), subLevel, level.Word.ToList());
            exam.Puan = level.levelPuan;

            exam.Star = progress == null ? 0 : progress.star;
            exam.TotalPuan = progress == null ? 0 : progress.puan;
            return Tuple.Create(exam, level);
        }

        public Tuple<Level, levelUserProgress> GetExamLevelStart(int levelNumber, int kind)
        {
            Level l = entities.Level.Find(levelNumber, kind);
            levelUserProgress progress =
                entities.levelUserProgress.FirstOrDefault(
                    a => a.userId == GetUserId && a.levelNumber == l.levelNumber && a.kind == l.kind);

            return Tuple.Create(l, progress);
        }

        public bool UpdateUserProggress(levelUserProgress userProgress)
        {
            levelUserProgress l =
                entities.levelUserProgress.FirstOrDefault(
                    a =>
                        a.userId == GetUserId && a.levelNumber == userProgress.levelNumber &&
                        a.kind == userProgress.kind);

            if (l == null)
            {
                levelUserProgress levelUser = new levelUserProgress
                {
                    userId = GetUserId,
                    levelNumber = userProgress.levelNumber,
                    kind = userProgress.kind,
                    star = userProgress.star,
                    puan = userProgress.puan
                };

                entities.levelUserProgress.Add(levelUser);
                entities.SaveChanges();
            }
            else
            {
                l.star = userProgress.star;
                l.puan = userProgress.puan;

                entities.Entry(l).State = EntityState.Modified;
                entities.SaveChanges();
            }

            return true;
        }
    }
}