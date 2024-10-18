using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattyPie.Models.DTOs
{
    public class SubThreadDtos : ISubThreadDtos
    {
        public List<string> SubUserIds { get; set; }
        public string ThreadId { get; set; }
        public string Id { get; set; }
        public List<string> UserIds { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public List<SubThreadDtos> SubThreads { get; set; }
    }
}
