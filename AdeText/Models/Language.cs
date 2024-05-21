using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Models
{
    public class Language : ILanguage
    {
        public Language(Dictionary<string, SupportedLanguage> _supportedLanguages, string _etag)
        {
            SupportedLanguages = _supportedLanguages;
            ETag = _etag;
        }

        public Language(string _etag)
        {
            ETag = _etag;
        }
        public Dictionary<string, SupportedLanguage> SupportedLanguages { get; set; }
        public string ETag { get; set; }
    }
}
