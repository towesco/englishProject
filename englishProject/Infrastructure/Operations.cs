using AutoMapper;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public static int GetTotalLevelPuan(Level level)
        {
            int total = 0;

            for (int i = 1; i <= level.levelSubLevel; i++)
            {
                for (int a = 1; a <= level.Word.Count; a++)
                {
                    total += level.levelPuan * i;
                }
            }
            return total;
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

        /// <returns></returns>
        public List<BoxLevelUser> GetBoxs()
        {
            //tüm kutular çekiliyor
            List<Box> boxs = entities.Box.OrderBy(a => a.boxNumber).ToList();

            List<BoxLevelUser> boxLevelUsers = new List<BoxLevelUser>();

            foreach (var itemBox in boxs)
            {
                var userLevels = (from u in entities.levelUserProgress
                                  join l in entities.Level on u.levelId equals l.levelId
                                  where u.userId == GetUserId && u.boxId == itemBox.boxId
                                  select new CustomLevel { Level = l, Star = u.star }).ToList();

                BoxLevelUser b = new BoxLevelUser { Box = itemBox };

                if (!userLevels.Any())
                {
                    userLevels = new List<CustomLevel> { new CustomLevel { Level = itemBox.Level.First(), Star = 0 } };
                }
                else
                {
                    CustomLevel levelLast = userLevels.OrderByDescending(a => a.Level.levelNumber).First();

                    levelUserProgress userProgressLast =
                        entities.levelUserProgress.First(
                            a =>
                                a.levelId == levelLast.Level.levelId);

                    if (userProgressLast.star >= 1)
                    {
                        Level levelNext =
                            entities.Level.FirstOrDefault(
                                a =>
                                    a.levelNumber == levelLast.Level.levelNumber + 1 && a.boxId == itemBox.boxId);

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

        #region Levels

        public Level GetLevel(int levelId)
        {
            return entities.Level.Find(levelId);
        }

        /// <summary>
        /// Bir sonraki seviyeyi getirir
        /// </summary>
        /// <param name="levelId"></param>

        /// <returns></returns>
        public Level GetNextLevel(int levelId)
        {
            Level l = GetLevel(levelId);
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
                        a.levelNumber == level.levelNumber + 1 && a.boxId == level.boxId);
        }

        #endregion Levels

        public Level GetWords(int levelId)
        {
            return entities.Level.Include("Word").First(a => a.levelId == levelId);
        }

        public UserDetail GetUserDetail()
        {
            return entities.UserDetail.FirstOrDefault(a => a.userId == GetUserId);
        }

        public double GetUserTargetPercent()
        {
            Score t = entities.Score.FirstOrDefault(a => a.userId == GetUserId && SqlFunctions.DateDiff("DAY", a.targetDate, DateTime.Now) == 0);

            if (t != null)
            {
                UserDetail userDetail = entities.UserDetail.Find(GetUserId);

                double percent = Math.Round(((double)t.targetScore / (double)userDetail.DailyTargetScore) * 100);

                return percent;
            }
            else
            {
                return 0;
            }
        }

        public void UpdateScore(int puan)
        {
            Score score = entities.Score.FirstOrDefault(a => a.userId == GetUserId && SqlFunctions.DateDiff("DAY", a.targetDate, DateTime.Now) == 0);

            if (score != null)
            {
                score.targetScore = score.targetScore + puan;
            }
            else
            {
                Score s = new Score() { targetDate = DateTime.Now, targetScore = puan, userId = GetUserId };
                entities.Score.Add(s);
            }
            entities.SaveChanges();
        }

        #region WordModulPictureModul

        /// <summary>
        /// Karışık soru getirir
        /// </summary>
        /// <param name="words">Sorulacak kelimeler</param>
        /// <param name="subLevel">Alt level belirtilir</param>
        /// <param name="questionsList">Şıklarda çıkacak sorular Default olarak sorulacak kelimelerden seçilir</param>
        /// <returns></returns>
        private WordModul GetWordModuleQuestionshuffle(IEnumerable<Word> words, ModulSubLevel subLevel, List<Word> questionsList)
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
                            QestionsOptions = siklar.OrderBy(a => rnd.Next()).ToList(),
                            QuestionRemender = item.wordRemender
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

            switch (subLevel)
            {
                case ModulSubLevel.Temel:

                    foreach (var item in words)
                    {
                        //1 tane doğru cevap liste
                        List<Word> correctListOne = new List<Word> { item };
                        //4 tane yanlış evap
                        List<string> answers = words.Except(correctListOne).OrderBy(a => rnd.Next()).ToList().Take(4).Select(a => a.wordTranslate).ToList();
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

        public Tuple<WordModul, Level> GetWordModul(ModulSubLevel subLevel, int levelId)
        {
            Level level = entities.Level.Find(levelId);
            levelUserProgress progress =

                entities.levelUserProgress.FirstOrDefault(
                  a => a.userId == GetUserId && a.levelId == level.levelId);

            List<Word> words = level.Word.ToList();

            WordModul exam = GetWordModuleQuestionshuffle(words, subLevel, words);

            exam.Puan = level.levelPuan;

            exam.Star = progress == null ? 0 : progress.star;
            exam.TotalPuan = progress == null ? 0 : progress.puan;
            return Tuple.Create(exam, level);
        }

        public Tuple<PictureWordModul, Level> GetPictureWordModul(ModulSubLevel subLevel, int levelId)
        {
            Level level = entities.Level.Find(levelId);
            levelUserProgress progress =

                entities.levelUserProgress.FirstOrDefault(
                  a => a.userId == GetUserId && a.levelId == level.levelId);

            List<Word> words = level.Word.ToList();
            PictureWordModul exam = GetPictureWordModuleQuestionshuffle(words, subLevel);
            exam.Puan = level.levelPuan;
            exam.Star = progress == null ? 0 : progress.star;
            exam.TotalPuan = progress == null ? 0 : progress.puan;
            return Tuple.Create(exam, level);
        }

        #endregion WordModulPictureModul

        /// <summary>
        /// Kutulardaki levellara tıklandığında gelen modalda hangi alt levelların  kapalı veya açık olduğunu belirtir.
        /// </summary>
        /// <param name="levelId"></param>

        /// <returns></returns>
        public Tuple<Level, levelUserProgress, levelUserProgress, List<Word>> GetExamLevelStart(int levelId)
        {
            //Günce seviye bilgileri
            Level l = entities.Level.Find(levelId);
            //Güncel LevelUser bilgileri
            levelUserProgress progress = entities.levelUserProgress.FirstOrDefault(a => a.userId == GetUserId && a.levelId == l.levelId);

            //bir önceki level bilgileri
            Level previousLevel = entities.Level.FirstOrDefault(a => a.levelNumber == l.levelNumber - 1 && a.boxId == l.boxId);

            //bir öncekli leveluser bilgileri
            levelUserProgress previousLevelUserProgress = null;

            if (previousLevel != null)
            {
                previousLevelUserProgress = entities.levelUserProgress.FirstOrDefault(a => a.levelId == previousLevel.levelId && a.userId == GetUserId);
            }

            return Tuple.Create(l, progress, previousLevelUserProgress, l.Word.ToList());
        }

        /// <summary>
        /// Üye test çözerken alt levelları başarılı bitirdiğinde veri tabanında güncelleme yapılmaktadır.
        /// </summary>
        /// <param name="userProgress"></param>
        /// <returns></returns>
        public bool UpdateUserProggress(levelUserProgress userProgress)
        {
            UpdateScore(userProgress.targetScore);

            levelUserProgress l =
                entities.levelUserProgress.FirstOrDefault(
                    a =>
                        a.userId == GetUserId && a.levelId == userProgress.levelId);

            if (l == null)
            {
                levelUserProgress levelUser = new levelUserProgress
                {
                    userId = GetUserId,
                    levelId = userProgress.levelId,
                    star = userProgress.star,
                    puan = userProgress.puan,
                    boxId = userProgress.boxId
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

        /// <summary>
        /// Kutuların ismini ve  bu kutulara bağlı level sayısını çekmektedir
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetBoxMenu()
        {
            Dictionary<string, int> result = entities.Box.ToDictionary(box => box.boxName, box => box.Level.Count);

            return result;
        }

        public void SignOut()
        {
            Authen.SignOut();
            HttpContext.Current.Session.Abandon();
        }

        public string UpdateUser(UserViewModel userr)
        {
            UserApp user = usermanager.FindById(GetUserId);

            user.Name = userr.Name;
            user.SurName = userr.SurName;
            user.Gender = userr.Gender;
            user.City = userr.City;

            if (!string.IsNullOrEmpty(userr.BirthDay))
            {
                user.BirthDay = DateTime.Parse(userr.BirthDay);
            }

            user.PicturePath = userr.PicturePath;

            user.PhoneNumber = userr.PhoneNumber;
            user.Email = userr.Email;
            user.UserName = userr.UserName;

            if (!string.IsNullOrEmpty(userr.Password))
            {
                usermanager.AddPassword(GetUserId, userr.Password);
            }

            IdentityResult result = usermanager.Update(user);

            if (result.Succeeded)
            {
                return true.ToString();
            }
            else
            {
                if (result.Errors.First().Contains("Email"))
                {
                    return "Bu email adresi kullanılmaktadır.";
                }
                else if (result.Errors.First().Contains("Name"))
                {
                    return "Bu kullanıcı ismi kullanılmaktadır.";
                }
                else
                {
                    return "Bir hata meydana geldi. lütfen daha sonra tekrar deneyiniz";
                }
            }
        }

        public string GetUpdateUserPicture(HttpPostedFileBase fileUpload)
        {
            UserApp user = usermanager.FindById(GetUserId);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
            fileUpload.SaveAs(HttpContext.Current.Server.MapPath("~/Pictures/UserPicture/" + fileName));

            if (!user.PicturePath.Contains("user") && !user.PicturePath.Contains("http"))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(user.PicturePath)))
                {
                    File.Delete((HttpContext.Current.Server.MapPath(user.PicturePath)));
                }
            }

            user.PicturePath = "/Pictures/UserPicture/" + fileName;
            usermanager.Update(user);
            return "/Pictures/UserPicture/" + fileName;
        }

        public string UpdateUserPassword(UserPasswordViewModel pass)
        {
            UserApp user = usermanager.FindById(GetUserId);

            IdentityResult result = usermanager.ChangePassword(GetUserId, pass.CurrentPassword, pass.NewPassword);
            if (result.Succeeded)
            {
                return "1";
            }
            else
            {
                return result.Errors.First();
            }
        }
    }
}