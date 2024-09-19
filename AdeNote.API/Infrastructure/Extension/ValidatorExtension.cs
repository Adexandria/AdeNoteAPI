using FluentValidation;

namespace AdeNote.Infrastructure.Extension
{
    public static class ValidatorExtension
    {
        public static IRuleBuilderOptions<T,string> IsValidTicketStatus<T> (this IRuleBuilder<T,string> ruleBuilder)
        {
            return ruleBuilder.Must(x =>
             {
                 var isValidStatus = new[] { "Pending", "Inreview", "Resolved", "Unresolved" };
                 if (isValidStatus.Contains(x))
                 {
                     return true;
                 }
                 return false;
             });
        }
    }
}
