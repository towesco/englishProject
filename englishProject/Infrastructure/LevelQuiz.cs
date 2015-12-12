using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure
{
    public class LevelExam
    {
        public int Puan { get; set; }

        public int TotalPuan { get; set; }

        public int Star { get; set; }

        public SubLevel SubLevel { get; set; }

        public List<Questions> Questions { get; set; }
    }

    public class Questions
    {
        public string Question { get; set; }

        public string QuestionCorrect { get; set; }

        public List<string> QestionsOptions { get; set; }
    }
}