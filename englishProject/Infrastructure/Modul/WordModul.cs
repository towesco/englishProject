using englishProject.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure
{
    public class WordModul
    {
        public int Puan { get; set; }

        public int TotalPuan { get; set; }

        public int Star { get; set; }

        public ModulSubLevel SubLevel { get; set; }

        public List<Questions> Questions { get; set; }
    }

    public class Questions
    {
        public string Question { get; set; }

        public string QuestionCorrect { get; set; }

        public string QuestionRemender { get; set; }

        public string Definition { get; set; }

        public string Example { get; set; }

        public List<string> QestionsOptions { get; set; }
    }
}