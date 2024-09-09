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


        public static MapifyConfiguration TicketStreamConfig()
        {
            return new MapifyConfigurationBuilder<TicketStreamDto, Ticket>()
                .Map(d => d.Issue, s => s.Issue)
                .Map(d => d.Description, s => s.Description)
                .CreateConfig();
        }

        public static MapifyConfiguration TicketConfig()
        {
            return new MapifyConfigurationBuilder<Ticket, TicketDTO>()
                    .Map(d => d.TicketId, s => s.Id)
                    .Map(d=>d.FirstName, s=>s.User.FirstName)
                    .Map(d=>d.LastName, s=>s.User.LastName)
                    .Map(d => d.Status, s => s.Status.GetDescription())
                     .Map(d => d.Created, s => s.Created.ToLongDateString())
                .CreateConfig();
        }

        public static MapifyConfiguration UserTicketConfig()
        {
            return new MapifyConfigurationBuilder<Ticket, UserTicketDto>()
                    .Map(d => d.Status, s => s.Status.GetDescription())
                     .Map(d => d.Created, s => s.Created.ToLongDateString())
                     .Map(d => d.Modified, s => s.Modified.ToLongDateString())
                .CreateConfig();
        }

        public static MapifyConfiguration TicketAdminConfig()
        {
            return new MapifyConfigurationBuilder<User, TicketDTO>()
                .Ignore(d=>d.FirstName)
                .Ignore(d=>d.LastName)
               .Map(d=>d.Handler, s=> $"{s.FirstName} {s.LastName}")
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

        public static MapifyConfiguration TicketsConfig()
        {
            return new MapifyConfigurationBuilder<Ticket,TicketsDTO>()
                .Map(d=>d.TicketId, s=>s.Id)
                .Map(d=>d.Status, s=>s.Status.GetDescription())
                .Map(d=>d.FirstName, s=>s.User.FirstName)
                .Map(d=>d.LastName, s=>s.User.LastName)
                .Map(d=>d.Created, s=>s.Created.ToLongDateString())
                .CreateConfig();
        }
    }
}
