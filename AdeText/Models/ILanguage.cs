using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Models
{
    public interface ILanguage
    {
        public Dictionary<string, string> TranslationLanguages { get; set; }

        public Dictionary<string, string> TransliterationLanguages { get; set; }

        public string ETag { get; set; }
    }
}
