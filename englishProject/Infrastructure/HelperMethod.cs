using englishProject.Infrastructure;
using englishProject.Models;
using Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace englishProject.Infrastructure
{
    public class HelperMethod
    {
        public static string GetErrorMessage
        {
            get { return "Hata meydana geldi. Lütfen daha sonra tekrar deneyiniz."; }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EnglishProjectDBEntities entities;

        public HelperMethod()
        {
            entities = new EnglishProjectDBEntities();
        }

        /// <summary>
        /// Genel Dil seçeneklerini getirir İngilizce=1,Almanca 2
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Level tablosundan  levelName ve levelın ait olduğu boxName çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetLevelSelectListItems()
        {
            return
                 entities.Level.OrderBy(a => a.levelId).ToList()
                     .Select(
                         a =>
                             new SelectListItem
                             {
                                 Text = String.Format("{0}({2})---->{1} kutusu", a.levelName, a.Box.boxName, Enum.GetName(typeof(Modul), a.levelModul) ?? ""),
                                 Value = a.levelId.ToString(CultureInfo.InvariantCulture),
                                 Selected = false
                             })
                     .ToList();
        }

        /// <summary>
        /// Box tablosundan boxName ve BoxNumber çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetBoxSelectListItems()
        {
            return entities.Box.ToList().Select(a => new SelectListItem
            {
                Text = a.boxName,
                Value = a.boxId.ToString(CultureInfo.InvariantCulture),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// Level tablosundaki LevelNumberAppear değeri için 1 den 50 kadar sayı çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetLevelNumberListItems()
        {
            return
                Enumerable.Range(1, 50)
                    .Select(a => new SelectListItem { Text = a.ToString(CultureInfo.InvariantCulture), Value = a.ToString(CultureInfo.InvariantCulture), Selected = false })
                    .ToList();
        }

        /// <summary>
        /// 1-10 kadar sayı getirir aynı zamanda eşleşen modul adlarını getirir
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetModulListItems()
        {
            return
                Enumerable.Range(1, 10)
                    .Select(a => new SelectListItem { Text = String.Format("{0}-{1}", a.ToString(CultureInfo.InvariantCulture), Enum.GetName(typeof(Modul), a) ?? "boş"), Value = a.ToString(CultureInfo.InvariantCulture), Selected = false })
                    .ToList();
        }

        public List<SelectListItem> GetCityListItems()
        {
            return entities.City.Select(a => new SelectListItem() { Text = a.Name, Value = a.Id.ToString() }).ToList();
        }

        public static string getGooglePicture(string key)
        {
            WebClient web = new WebClient();

            string json = web.DownloadString(
                      "https://www.googleapis.com/plus/v1/people/117697405997139435772?fields=image&key=" + key);

            JObject j = JObject.Parse(json);

            ;

            return (string)j["image"]["url"];
        }

        #region BilgilendirmeMesaj

        /// <summary>
        /// Sitede gösterilen mesajlar
        /// </summary>
        /// <param name="info">bilgilendirme yazısı</param>
        /// <param name="alert"></param>
        /// <param name="key">Numarası dikkatli olmak lazım daha önceki verdiğimiz numaralar olmamalı</param>
        /// <returns></returns>
        public static MvcHtmlString GetMessage(IHtmlString info, Alert alert, int key)
        {
            HttpCookie h = HttpContext.Current.Request.Cookies["alert"];

            string data = String.Empty;

            if (h != null)
            {
                if (h[key.ToString(CultureInfo.InvariantCulture)] == null || h[key.ToString(CultureInfo.InvariantCulture)] == true.ToString())
                {
                    h[key.ToString(CultureInfo.InvariantCulture)] = true.ToString();
                    h.Expires = DateTime.Now.AddDays(60);
                    HttpContext.Current.Response.Cookies.Add(h);
                    data = String.Format(
                        "<div class='alert alert-{0} alert-dismissible notRadius' role='alert'><button data-key='{2}' type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>{1}</div>",
                        alert.ToString(), info.ToHtmlString(), key);
                }
            }
            else
            {
                h = new HttpCookie("alert");
                h[key.ToString(CultureInfo.InvariantCulture)] = true.ToString();
                h.Expires = DateTime.Now.AddDays(60);
                HttpContext.Current.Response.Cookies.Add(h);
                data = String.Format(
                    "<div class='alert alert-{0} alert-dismissible notRadius' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>{1}</div>",
                    alert.ToString(), info.ToHtmlString());
            }

            return new MvcHtmlString(data);
        }

        /// <summary>
        /// cookie sayesinde mesajı saklar. ve bir daha göstermez
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetMessageHide(string key)
        {
            HttpCookie h = HttpContext.Current.Request.Cookies["alert"];
            if (h != null)
            {
                h[key] = h[key].Replace(true.ToString(), false.ToString());

                HttpContext.Current.Response.Cookies.Set(h);
            }
            return true;
        }

        public static string getUserName(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                email = email.Substring(0, email.IndexOf('@'));
                if (email.Length > 16)
                {
                    email = email.Substring(0, 13) + "...";
                }
            }
            else
            {
                if (email.Length > 16)
                {
                    email = email.Substring(0, 13) + "...";
                }
            }

            return email;
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

        #endregion BilgilendirmeMesaj
    }
}