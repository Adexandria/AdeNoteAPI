using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services
{
    public class StatisticService : IStatisticsService
    {
        private readonly IUserRepository userRepository;
        private readonly ITicketRepository ticketRepository;

        public StatisticService(IUserRepository _userRepository, ITicketRepository _ticketRepository)
        {
           userRepository = _userRepository;
            ticketRepository = _ticketRepository;
        }
        public ActionResult<StatisticsDto> GetStatistics()
        {
            var numberOfUsers = userRepository.GetNumberOfUsers();
            var numberOfTickets = ticketRepository.GetNumberOfTicketsByStatus();

            return ActionResult<StatisticsDto>.SuccessfulOperation(new StatisticsDto(numberOfUsers,numberOfTickets));
        }
    }
}
