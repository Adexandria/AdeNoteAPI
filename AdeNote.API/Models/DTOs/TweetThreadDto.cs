﻿namespace AdeNote.Models.DTOs
{
    public class TweetThreadDto

    {
        public Guid Id { get; set; }
        public List<string> UserNames { get; set; }

        public string Message { get; set; }

        public string Date {  get; set; }

        public string LastModified { get; set; }

        public List<SubThreadDto> Comments { get; set; }
    }
}
