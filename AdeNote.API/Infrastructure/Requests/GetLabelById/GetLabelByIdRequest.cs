using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetLabelById
{
    public class GetLabelByIdRequest: IRequest<ActionResult<LabelDTO>>
    {
        public Guid LabelId { get; set; }
    }
}
