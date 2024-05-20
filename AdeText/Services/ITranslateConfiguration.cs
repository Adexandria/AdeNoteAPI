﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeText.Services
{
    public interface ITranslateConfiguration
    {
        public string Key { get; set; }
        public string Location { get; set; }
        public string Endpoint { get; set; }
        public int RetryConfiguration { get; set; }
    }
}
