namespace AdeNote.Models.DTOs
{
    public class StatisticsDto
    {
        public StatisticsDto(int numberOfUsers,
            int numberOfTicketsPending,
            int numberOfTicketsInReview,
            int numberOfTicketsClosed,
            int numberofTicketsUnresolved
            )
        {
            Users = numberOfUsers;
            TicketInreview = numberOfTicketsInReview;
            TicketPending = numberOfTicketsPending;
            TicketUnResolved = numberofTicketsUnresolved;
            TicketClosed = numberOfTicketsClosed;
            TotalTickets = TicketInreview + TicketPending + TicketClosed + TicketUnResolved;

        }
        public int Users { get; }

        public int TicketPending { get; }

        public int TicketInreview { get; }

        public int TicketUnResolved { get; }

        public int TicketClosed { get; }

        public int TotalTickets {  get; }
    }
}
