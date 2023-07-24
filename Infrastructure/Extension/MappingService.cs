using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;

namespace AdeNote.Infrastructure.Extension
{
    public static class MappingService
    {
        public static TypeAdapterConfig BookConfig()
        {
            return TypeAdapterConfig<Book, BookDTO>
                .ForType().Map(d=> d.BookPages, s=>s.Pages).Config;
        }

        public static TypeAdapterConfig LabelConfig()
        {
            return TypeAdapterConfig<Label, LabelDTO>
                .ForType().Map(d=> d.Label,s=>s.Title)
                .Config;
        }

        public static TypeAdapterConfig PageLabelsConfig()
        {
            return TypeAdapterConfig<Page, PageDTO>
                .ForType().Map(d => d.Labels, s => s.Labels)
                .Config;
        }

        public static TypeAdapterConfig UpdateLabelConfig()
        {
            return TypeAdapterConfig<PageUpdateDTO, Page>
                .NewConfig().Ignore(d=>d.Labels).Config;
        }
    }
}
