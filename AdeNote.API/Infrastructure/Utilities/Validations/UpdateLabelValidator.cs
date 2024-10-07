using AdeNote.Infrastructure.Requests.UpdateLabel;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class UpdateLabelValidator : AbstractValidator<UpdateLabelRequest>
    {
        public UpdateLabelValidator()
        {
            RuleFor(s => s.LabelId).NotEmpty().WithMessage("Invalid label id");
            RuleFor(s => s.UpdateLabel).Custom((list, context) =>
            {
                var result = string.IsNullOrEmpty(list.Title);
                if(result)
                    context.AddFailure("Invalid title");
            });
        }
    }
}
