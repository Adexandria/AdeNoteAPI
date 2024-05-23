using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;

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
        public static TypeAdapterConfig BookConfig()
        {
            return TypeAdapterConfig<Book, BookDTO>
                .ForType().Map(d=> d.BookPages, s=>s.Pages).Config;
        }

        /// <summary>
        /// Maps from label to label dto
        /// </summary>
        /// <returns>a configuration type</returns>
        public static TypeAdapterConfig LabelConfig()
        {
            return TypeAdapterConfig<Label, LabelDTO>
                .ForType().Map(d=> d.Label,s=>s.Title)
                .Config;
        }

        /// <summary>
        /// Maps from page to PageDTO
        /// </summary>
        /// <returns>a configuration type</returns>
        public static TypeAdapterConfig PageLabelsConfig()
        {
            return TypeAdapterConfig<Page, PageDTO>
                .ForType().Map(d => d.Labels,
                s => s.Labels.Adapt<IList<LabelDTO>>(LabelConfig()))
                .Config;
        }

        /// <summary>
        /// Maps from page update dto to page
        /// </summary>
        /// <returns>a configuration type</returns>
        public static TypeAdapterConfig UpdateLabelConfig()
        {
            return TypeAdapterConfig<PageUpdateDTO, Page>
                .NewConfig().Ignore(d=>d.Labels).Config;
        }
    }
}
