namespace AdeNote.Models.DTOs
{
    public class SubThreadDto
    {
        public Guid Id { get; set; }
        public List<string> Usernames { get; set; }
        public List<string> ReplyUsernames { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public string LastModified { get; set; }
        public List<SubThreadDto> Comments {  get; set; }
    }
}
