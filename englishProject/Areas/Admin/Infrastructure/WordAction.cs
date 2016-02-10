using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Helpers;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class WordAction : IWord
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private wordboxe_englishEntities entities;

        public WordAction()
        {
            entities = new wordboxe_englishEntities();
        }

        public bool AddWord(Word word)
        {
            if (entities.Word.Any(a => a.wordTurkish.Trim() == word.wordTurkish.Trim()))
            {
                return false;
            }
            else
            {
                entities.Entry(word).State = EntityState.Added;
                entities.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Word> Words(int levelId)
        {
            try
            {
                return entities.Word.Where(a => a.levelId == levelId).OrderByDescending(a => a.wordId).ToList();
            }
            catch (Exception)
            {
                return new List<Word>();
            }
        }

        public bool UpdateWord(Word word)
        {
            entities.Entry(word).State = EntityState.Modified;
            entities.SaveChanges();
            return true;
        }

        public bool DeleteWord(int wordId)
        {
            Word w = entities.Word.First(a => a.wordId == wordId);

            entities.Word.Remove(w);

            if (File.Exists(HttpContext.Current.Server.MapPath(w.wordRemender)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(w.wordRemender));
            }

            if (File.Exists(HttpContext.Current.Server.MapPath(w.wordRemenderInfo)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(w.wordRemenderInfo));
            }

            entities.SaveChanges();
            return true;
        }

        public bool DeletePicture(string path)
        {
            try
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(path)))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(path));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Word GetWord(int wordId)
        {
            return entities.Word.First(a => a.wordId == wordId);
        }

        public IEnumerable<Level> GetLevels()
        {
            return entities.Level.ToList();
        }
    }
}