using System.Collections.Generic;

namespace englishProject.Infrastructure
{
    public class ScoreTableBox
    {
        public string BoxName { get; set; }

        public int boxTotalScore { get; set; }

        public List<ScoreTableLevel> ScoreTableLevels { get; set; }
    }

    public class ScoreTableLevel
    {
        public int LevelNumber { get; set; }

        public string LevelName { get; set; }

        public int Score { get; set; }
    }
}