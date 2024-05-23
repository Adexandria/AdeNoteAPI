using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Models
{
    public class TextLanguage : ILanguage
    {
        public TextLanguage(Dictionary<string, string> _supportedLanguages, string _etag)
        {
            SupportedLanguages = _supportedLanguages;
            ETag = _etag;
        }

        public TextLanguage(string _etag)
        {
            ETag = _etag;
        }
        public Dictionary<string, string> SupportedLanguages { get; set; }
        public string ETag { get; set; }
    }
}
