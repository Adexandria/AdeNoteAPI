namespace AdeNote.Models.DTOs
{
    public class TicketStatusDto
    {
        public TicketStatusDto(Status status, int numberOfTickets)
        {
            Status = status.ToString();
            NumberOfTickets = numberOfTickets;
        }
        public string Status {  get;}

        public int NumberOfTickets {  get;}
    }
}
