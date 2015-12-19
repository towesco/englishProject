﻿using englishProject.Infrastructure.Users;
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

        /// <summary>
        /// User/Index sayfasındaki boxs ları levelları ile beraber çeker
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public List<BoxLevelUser> GetBoxs(Kind kind)
        {
            int _kind = (int)kind;

            //tüm kutular çekiliyor
            List<Box> boxs = entities.Box.Include("Level").Where(a => a.kind == _kind).OrderBy(a => a.boxNumber).ToList();

            List<BoxLevelUser> boxLevelUsers = new List<BoxLevelUser>();

            foreach (var itemBox in boxs)
            {
                var userLevels = (from u in entities.levelUserProgress
                                  join l in entities.Level on new { u.levelNumber, u.kind } equals new { l.levelNumber, l.kind }
                                  where u.userId == GetUserId && u.boxNumber == itemBox.boxNumber
                                  select new CustomLevel { Level = l, Star = u.star }).ToList();

                BoxLevelUser b = new BoxLevelUser { Box = itemBox };

                if (!userLevels.Any())
                {
                    userLevels = new List<CustomLevel> { new CustomLevel { Level = itemBox.Level.First(), Star = 0 } };
                }
                else
                {
                    CustomLevel levelLast = userLevels.OrderByDescending(a => a.Level.levelNumberAppear).First();

                    levelUserProgress userProgressLast =
                        entities.levelUserProgress.First(
                            a =>
                                a.levelNumber == levelLast.Level.levelNumber && a.kind == levelLast.Level.kind &&
                                a.boxNumber == levelLast.Level.boxNumber);

                    if (userProgressLast.star >= 1)
                    {
                        Level levelNext =
                            entities.Level.FirstOrDefault(
                                a =>
                                    a.levelNumberAppear == levelLast.Level.levelNumberAppear + 1 && a.kind == levelLast.Level.kind &&
                                    a.boxNumber == levelLast.Level.boxNumber);

                        if (levelNext != null)
                        {
                            userLevels.Add(new CustomLevel { Level = levelNext, Star = 0 });
                        }
                    }
                }

                b.UserLevels = userLevels;

                b.OtherLevels = itemBox.Level.Except(b.UserLevels.Select(a => a.Level)).ToList();

                boxLevelUsers.Add(b);
            }

            return boxLevelUsers;
        }

        /// <summary>
        /// Üye olan kullanıcının UserApp bilgisini verir
        /// </summary>
        /// <returns></returns>
        public UserApp getProfil()
        {
            return usermanager.FindById(GetUserId);
        }

        public Level GetLevel(int levelNumber, int kind)
        {
            return entities.Level.First(a => a.levelNumber == levelNumber && a.kind == kind);
        }

        /// <summary>
        /// Bir sonraki seviyeyi getirir
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public Level GetNextLevel(int levelNumber, int kind)
        {
            Level l = GetLevel(levelNumber, kind);
            return GetNextLevel(l);
        }

        /// <summary>
        /// Bir sonraki seviyeyi getirir
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Level GetNextLevel(Level level)
        {
            return
                entities.Level.FirstOrDefault(
                    a =>
                        a.levelNumberAppear == level.levelNumberAppear + 1 && a.kind == level.kind &&
                        a.boxNumber == level.boxNumber);
        }

        #region WordModul

        /// <summary>
        /// Karışık soru getirir
        /// </summary>
        /// <param name="words">Sorulacak kelimeler</param>
        /// <param name="subLevel">Alt level belirtilir</param>
        /// <param name="questionsList">Şıklarda çıkacak sorular Default olarak sorulacak kelimelerden seçilir</param>
        /// <returns></returns>
        private WordModul GetWordModuleQuestionshuffle(IEnumerable<Word> words, ModulSubLevel subLevel, IEnumerable<Word> questionsList)
        {
            Random rnd = new Random();
            List<Questions> List = new List<Questions>();
            List<string> siklar;

            switch (subLevel)
            {
                case ModulSubLevel.Temel:

                    foreach (var item in words)
                    {
                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word> { item };
                        //4 tane yanlış evap
                        siklar = questionsList.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTurkish).ToList();
                        //1 tane doğru cevap şıklar ekleniyor
                        siklar.Add(item.wordTurkish);

                        Questions q = new Questions
                        {
                            Question = item.wordTranslate,
                            QuestionCorrect = item.wordTurkish,
                            QestionsOptions = siklar.OrderBy(a => rnd.Next()).ToList()
                        };

                        List.Add(q);
                    }

                    break;

                case ModulSubLevel.İleri:

                    foreach (var item in words)
                    {
                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word> { item };
                        //4 tane yanlış evap
                        siklar = questionsList.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTranslate).ToList();
                        //1 tane doğru cevap şıklar ekleniyor
                        siklar.Add(item.wordTranslate);

                        Questions q = new Questions
                        {
                            Question = item.wordTurkish,
                            QuestionCorrect = item.wordTranslate,
                            QestionsOptions = siklar.OrderBy(a => rnd.Next()).ToList()
                        };

                        List.Add(q);
                    }

                    break;

                case ModulSubLevel.Mükemmel:

                    List.AddRange(words.Select(item => new Questions { Question = item.wordTurkish, QuestionCorrect = item.wordTranslate }));

                    break;
            }

            WordModul exam = new WordModul { SubLevel = subLevel, Questions = List.OrderBy(a => rnd.Next()).ToList() };

            return exam;
        }

        private PictureWordModul GetPictureWordModuleQuestionshuffle(List<Word> words, ModulSubLevel subLevel)
        {
            Random rnd = new Random();
            List<PictureQuestions> List = new List<PictureQuestions>();
            List<string> answers = null;

            switch (subLevel)
            {
                case ModulSubLevel.Temel:

                    foreach (var item in words)
                    {
                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word> { item };
                        //4 tane yanlış evap
                        answers = words.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTranslate).ToList();
                        //1 tane doğru cevap şıklar ekleniyor
                        answers.Add(item.wordTranslate);

                        PictureQuestions q = new PictureQuestions
                        {
                            QuestionCorrect = item.wordTranslate,
                            QestionsOptions = answers.OrderBy(a => rnd.Next()).ToList(),
                            QuestionInfo = item.info,
                            QuestionPicture = item.picture
                        };

                        List.Add(q);
                    }

                    break;

                case ModulSubLevel.İleri:

                    List.AddRange(words.Select(item => new PictureQuestions { QuestionCorrect = item.wordTranslate, QuestionInfo = item.info, QuestionPicture = item.picture }));

                    break;
            }

            PictureWordModul exam = new PictureWordModul { SubLevel = subLevel, Questions = List.OrderBy(a => rnd.Next()).ToList() };

            return exam;
        }

        public Tuple<WordModul, Level> GetWordModul(ModulSubLevel subLevel, int levelNumber, int kind)
        {
            Level level = entities.Level.Find(levelNumber, kind);
            levelUserProgress progress =

                entities.levelUserProgress.FirstOrDefault(
                  a => a.userId == GetUserId && a.levelNumber == level.levelNumber && a.kind == level.kind);

            IEnumerable<Word> words = entities.Word.Where(a => a.levelNumber == levelNumber && a.kind == kind).ToList();

            WordModul exam = GetWordModuleQuestionshuffle(words, subLevel, words);

            exam.Puan = level.levelPuan;

            exam.Star = progress == null ? 0 : progress.star;
            exam.TotalPuan = progress == null ? 0 : progress.puan;
            return Tuple.Create(exam, level);
        }

        public Tuple<PictureWordModul, Level> GetPictureWordModul(ModulSubLevel subLevel, int levelNumber, int kind)
        {
            Level level = entities.Level.Find(levelNumber, kind);
            levelUserProgress progress =

                entities.levelUserProgress.FirstOrDefault(
                  a => a.userId == GetUserId && a.levelNumber == level.levelNumber && a.kind == level.kind);
            List<Word> words = entities.Word.Where(a => a.levelNumber == levelNumber && a.kind == kind).ToList();
            PictureWordModul exam = GetPictureWordModuleQuestionshuffle(words, subLevel);
            exam.Puan = level.levelPuan;
            exam.Star = progress == null ? 0 : progress.star;
            exam.TotalPuan = progress == null ? 0 : progress.puan;
            return Tuple.Create(exam, level);
        }

        #endregion WordModul

        /// <summary>
        /// Kutulardaki levellara tıklandığında gelen modalda hangi alt levelların  kapalı veya açık olduğunu belirtir.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public Tuple<Level, levelUserProgress, levelUserProgress> GetExamLevelStart(int levelNumber, int kind)
        {
            //Günce seviye bilgileri
            Level l = entities.Level.Find(levelNumber, kind);
            //Güncel LevelUser bilgileri
            levelUserProgress progress = entities.levelUserProgress.FirstOrDefault(a => a.userId == GetUserId && a.levelNumber == l.levelNumber && a.kind == l.kind && a.boxNumber == l.boxNumber);

            //bir önceki level bilgileri
            Level previousLevel = entities.Level.FirstOrDefault(a => a.levelNumberAppear == l.levelNumberAppear - 1 && a.kind == l.kind && a.boxNumber == l.boxNumber);

            //bir öncekli leveluser bilgileri
            levelUserProgress previousLevelUserProgress = null;

            if (previousLevel != null)
            {
                previousLevelUserProgress = entities.levelUserProgress.FirstOrDefault(a => a.levelNumber == previousLevel.levelNumber && a.kind == previousLevel.kind && a.userId == GetUserId && a.boxNumber == previousLevel.boxNumber);
            }

            return Tuple.Create(l, progress, previousLevelUserProgress);
        }

        /// <summary>
        /// Üye test çözerken alt levelları başarılı bitirdiğinde veri tabanında güncelleme yapılmaktadır.
        /// </summary>
        /// <param name="userProgress"></param>
        /// <returns></returns>
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
                    puan = userProgress.puan,
                    boxNumber = userProgress.boxNumber
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

        /// <summary>
        /// User/Index sayfasındaki kullanıcı hakkında bilgiler çekmektedir
        /// </summary>
        /// <returns></returns>
        public UserProfilView GetUserProfilViewMenu()
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
            var result = (from u in entities.levelUserProgress
                          join b in entities.Box on new { u.boxNumber, u.kind } equals new { b.boxNumber, b.kind }
                          group b by b.boxName
                              into g
                              select new UserProfilBox
                              {
                                  BoxName = g.Key,
                                  LevelCurrent = g.Count()
                              }).ToList();
            userProfilView.UserProfilBoxs = result;
            return userProfilView;
        }

        /// <summary>
        /// Kutuların ismini ve  bu kutulara bağlı level sayısını çekmektedir
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetBoxMenu()
        {
            Dictionary<string, int> result = entities.Box.ToDictionary(box => box.boxName, box => box.Level.Count);

            return result;
        }
    }
}