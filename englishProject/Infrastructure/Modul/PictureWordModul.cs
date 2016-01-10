using englishProject.Infrastructure;
using System.Collections.Generic;

namespace englishProject.Infrastructure
{
    public class PictureWordModul
    {
        public int Puan { get; set; }

        public int TotalPuan { get; set; }

        public int Star { get; set; }

        public ModulSubLevel SubLevel { get; set; }

        public List<PictureQuestions> Questions { get; set; }
    }

    public class PictureQuestions : Questions
    {
        public string QuestionInfo { get; set; }

        public string QuestionPicture { get; set; }
    }
}