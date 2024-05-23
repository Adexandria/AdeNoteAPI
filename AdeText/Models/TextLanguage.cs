using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Models
{
    public class TextLanguage : ILanguage
    {
        public TextLanguage(Dictionary<string, string> _translationLanguages,
            Dictionary<string,string> _transliterationLanguages,string _etag)
        {
            TranslationLanguages = _translationLanguages;
            TransliterationLanguages = _transliterationLanguages;
            ETag = _etag;
        }

        public TextLanguage(string _etag)
        {
            ETag = _etag;
        }
        public Dictionary<string, string> TranslationLanguages { get; set; }
        public Dictionary<string, string> TransliterationLanguages { get; set; }
        public string ETag { get; set; }
    }
}
