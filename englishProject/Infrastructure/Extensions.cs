using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Infrastructure
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString HtmlConvertToJson(this HtmlHelper htmlHelper,
        object model)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            }; return new HtmlString(JsonConvert.SerializeObject(model, settings));
        }
    }
}