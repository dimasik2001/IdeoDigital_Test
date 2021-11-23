using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdeoDigital_TestProject.Models;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace IdeoDigital_TestProject.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaReference(this IMedia media)
        {
            var refs = new List<MediaReferenceModel>();
            refs.Add(new MediaReferenceModel
            {
                Key = Guid.NewGuid().ToString(),
                MediaKey = media.Key.ToString()
            }
            );

            return JsonConvert.SerializeObject(refs);
        }
    }
}