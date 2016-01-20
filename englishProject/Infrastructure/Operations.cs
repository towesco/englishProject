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
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace englishProject.Infrastructure
{
    public class Operations
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public UserApp getProfil(string userId)
        {
            return usermanager.FindById(userId);
        }

        public Level GetWords(int levelId)
        {
            return entities.Level.Include("Word").First(a => a.levelId == levelId);
        }

        public List<ScoreChart> GetScoreChart()
        {
            UserDetail userDetail = GetUserDetail();

            DateTime oldDate = DateTime.Now.AddDays(-10);

            var result =
                entities.Score.Where(a => a.targetDate >= oldDate && a.targetDate <= DateTime.Now && a.userId == GetUserId)
                    .ToList();

            return result.Select(item => new ScoreChart { Date = item.targetDate.ToShortDateString(), CurrentScore = item.targetScore, TargetScore = userDetail.DailyTargetScore }).ToList();
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

        /// <summary>
        /// Kutuların ismini ve  bu kutulara bağlı level sayısını çekmektedir
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetBoxMenu()
        {
            Dictionary<string, int> result = entities.Box.ToDictionary(box => box.boxName, box => box.Level.Count);

            return result;
        }

        /// <summary>
        /// çıkış
        /// </summary>
        public void SignOut()
        {
            Authen.SignOut();
            HttpContext.Current.Session.Abandon();
        }

        #region Levels

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

        #region User

        /// <summary>
        /// Üye test çözerken alt levelları başarılı bitirdiğinde veri tabanında güncelleme yapılmaktadır.
        /// </summary>
        /// <param name="userProgress"></param>
        /// <returns></returns>
        public bool UpdateUserProggress(levelUserProgress userProgress)
        {
            try
            {
                //hata var
                UpdateScore(userProgress.TargetScore);

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
            catch (Exception ex)
            {
                logger.Error("UpdateUserProggress", ex);
                return false;
            }

            //hata var

            ;
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

        /// <summary>
        /// kullanıcı güncelleme
        /// </summary>
        /// <param name="userr"></param>
        /// <returns></returns>
        public string UpdateUser(UserViewModel userr)
        {
            try
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
                        return HelperMethod.GetErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("UpdateUser", ex);
                return HelperMethod.GetErrorMessage;
            }
        }

        /// <summary>
        /// kullanıcı resmi güncelleme
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public string GetUpdateUserPicture(HttpPostedFileBase fileUpload)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
            try
            {
                UserApp user = usermanager.FindById(GetUserId);

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
            }
            catch (Exception ex)
            {
                logger.Error("GetUpdateUserPicture", ex);
            }

            return "/Pictures/UserPicture/" + fileName;
        }

        /// <summary>
        /// Kullanıcı password güncelleme
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string UpdateUserPassword(UserPasswordViewModel pass)
        {
            try
            {
                UserApp user = usermanager.FindById(GetUserId);
                IdentityResult result = usermanager.ChangePassword(GetUserId, pass.CurrentPassword, pass.NewPassword);
                if (result.Succeeded)
                {
                    return "true";
                }
                else
                {
                    return result.Errors.First();
                }
            }
            catch (Exception ex)
            {
                logger.Error("UpdateUserPassword", ex);
                return HelperMethod.GetErrorMessage;
            }
        }

        public UserDetail GetUserDetail()
        {
            return entities.UserDetail.FirstOrDefault(a => a.userId == GetUserId);
        }

        #endregion User

        #region Comments

        /// <summary>
        /// Seviyeye ait yorumlar
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public List<comment> GetComment(int levelId)
        {
            return entities.comment.Where(a => a.commentExceptId == levelId && a.commentReplyId == null).OrderByDescending(b => b.commentDate).ToList();
        }

        /// <summary>
        /// Tüm yorumlar
        /// </summary>
        /// <returns></returns>
        public List<CommentCustom> GetComment()
        {
            return (from c in entities.comment
                    join l in entities.Level on c.commentExceptId equals l.levelId
                    where c.commentReplyId == null
                    orderby c.commentDate descending
                    select new CommentCustom()
                    {
                        Comment = c,
                        Level = l
                    }).ToList();
        }

        #endregion Comments
    }
}