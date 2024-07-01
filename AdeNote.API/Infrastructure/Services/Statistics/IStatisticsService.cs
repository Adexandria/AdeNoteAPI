using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.Statistics
{
    public interface IStatisticsService
    {
        ActionResult<StatisticsDto> GetStatistics();
    }
}
