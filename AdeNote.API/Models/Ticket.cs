namespace AdeNote.Models
{
    public class Ticket : BaseClass
    {
        protected Ticket()
        {
            
        }

        public Ticket(Guid userId, string description)
        {
            Issuer = userId;
            Description = description;
            Status = Status.Pending;
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }

        public void SetImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
        }


        public void UpdateTicket(Guid adminId)
        {
            Modified = DateTime.UtcNow;
            AdminId = adminId;
        }

        public Guid Issuer { get; set; }

        public string Issue {  get; set; }

        public User User { get; set; }

        public Status Status { get; set; }

        public string? ImageUrl { get; set; }

        public string Description { get; set; }

        public Guid? AdminId { get; set; }
    }
}
