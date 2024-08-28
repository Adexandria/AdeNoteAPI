using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automapify.Services;
using Automapify.Services.Extensions;
using Automappify.Services;

namespace AdeNote.Infrastructure.Extension
{
    /// <summary>
    /// A custom configuration to map the entities into dto and vice versa
    /// </summary>
    public static class MappingService
    {
        /// <summary>
        /// Maps from book entity to book dto
        /// </summary>
        /// <returns>a configuration type</returns>
        public static MapifyConfiguration BookConfig()
        {
            return new MapifyConfigurationBuilder<Book, BookDTO>()
                .Map(d=> d.BookPages, s=>s.Pages)
                .CreateConfig();
        }

        /// <summary>
        /// Maps from label to label dto
        /// </summary>
        /// <returns>a configuration type</returns>
        public static MapifyConfiguration LabelConfig()
        {
            return new MapifyConfigurationBuilder<Label, LabelDTO>()
                .Map(d=> d.Label,s=>s.Title)
                .CreateConfig();
        }


        public static MapifyConfiguration TicketConfig()
        {
            return new MapifyConfigurationBuilder<TicketStreamDto, Ticket>()
                .Map(d => d.Issue, s => s.Issue)
                .Map(d => d.Description, s => s.Description)
                .CreateConfig();
        }
        /// <summary>
        /// Maps from page to PageDTO
        /// </summary>
        /// <returns>a configuration type</returns>
        public static MapifyConfiguration PageLabelsConfig()
        {
            return new MapifyConfigurationBuilder<Page, PageDTO>()
                .Map(d => d.Labels.MapTo(s=>s.Label),
                 s => s.Labels.MapFrom(s=>s.Title))
                .Map(d=>d.Labels.MapTo(s=>s.Id), s=>s.Labels.MapFrom(s=>s.Id))
                .CreateConfig();
        }
    }
}
