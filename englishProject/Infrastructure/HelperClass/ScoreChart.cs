using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.HelperClass
{
    public class ScoreChart
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("targetScore")]
        public int TargetScore { get; set; }

        [JsonProperty("currentScore")]
        public int CurrentScore { get; set; }
    }
}