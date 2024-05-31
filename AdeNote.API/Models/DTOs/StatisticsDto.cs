namespace AdeNote.Models.DTOs
{
    public class StatisticsDto
    {
        public StatisticsDto(int numberOfUsers,
            List<TicketStatusDto> numberOfTickets)
        {
            Users = numberOfUsers;
            Tickets = numberOfTickets;
            TotalTickets = Tickets.Sum(s => s.NumberOfTickets);
        }
        public int Users { get; }

        public List<TicketStatusDto> Tickets {  get; }

        public int TotalTickets {  get; }
    }
}
