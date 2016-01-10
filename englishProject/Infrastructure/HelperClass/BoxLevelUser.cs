using englishProject.Models;
using System.Collections.Generic;

namespace englishProject.Infrastructure
{
    public class BoxLevelUser
    {
        public Box Box { get; set; }

        public List<CustomLevel> UserLevels { get; set; }

        public List<Level> OtherLevels { get; set; }
    }

    public class CustomLevel
    {
        public Level Level { get; set; }

        public int Star { get; set; }
    }
}