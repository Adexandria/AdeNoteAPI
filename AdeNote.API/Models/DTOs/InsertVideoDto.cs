using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Models.DTOs
{
    public class InsertVideoDto
    {
        public IFormFile File { get; set; }

        public string Description { get; set; }
    }
}
