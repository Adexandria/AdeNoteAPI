using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Models
{
    public interface ILanguage
    {
        public Dictionary<string, SupportedLanguage> SupportedLanguages { get; set; }
        public string ETag { get; set; }
    }
}
