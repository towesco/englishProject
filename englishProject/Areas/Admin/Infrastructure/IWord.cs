using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public interface IWord
    {
        bool AddWord(Word word);

        IEnumerable<Word> Words(int levelId);

        bool UpdateWord(Word word);

        bool DeleteWord(int wordId);

        bool DeletePicture(string path);

        Word GetWord(int wordId);

        IEnumerable<Level> GetLevels();
    }
}