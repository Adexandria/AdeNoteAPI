using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.TranslatePage
{
    public class TranslatePageRequest: IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public Guid PageId { get; set; }
        public string TranslatedLanguage { get; set; }
    }
}
