using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.UpdateLabel
{
    public class UpdateLabelRequest : IRequest<ActionResult>
    {
        public Guid LabelId { get; set; }

        public LabelUpdateDTO UpdateLabel { get; set; }
    }
}
