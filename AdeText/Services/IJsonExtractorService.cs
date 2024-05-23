using AdeText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AdeText.Services
{
    internal interface IJsonExtractorService
    {
        T ExtractJson<T>(Stream json) where T : class, new();
    }
}
