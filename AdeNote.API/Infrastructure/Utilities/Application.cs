using FluentValidation;
using MediatR;

namespace AdeNote.Infrastructure.Utilities
{
    public class Application
    {
        public Application(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider; 
        }

        public async Task<ActionResult> SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<ActionResult>
        {
            var validator = serviceProvider.GetRequiredService<IValidator<TRequest>>()
                ?? throw new NullReferenceException("Invalid validator, Register validator");

            var validationResponse = validator.Validate(request);

            if (!validationResponse.IsValid)
            {
                var errorMessages = validationResponse.Errors.Select(s => s.ErrorMessage).ToList();

                return new ActionResult().AddErrors(errorMessages);
            }


            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, ActionResult>>()
                 ?? throw new NullReferenceException("Invalid request handler, Register request handler");

            var response = await handler.Handle(request, cancellationToken);

            return response;
        }

        public async Task<ActionResult<TResponse>> SendAsync<TRequest,TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest: IRequest<ActionResult<TResponse>>
        {
            var validator = serviceProvider.GetRequiredService<IValidator<TRequest>>()
                           ?? throw new NullReferenceException("Invalid validator, Register validator");

            var validationResponse = validator.Validate(request);

            if (!validationResponse.IsValid)
            {
                var errorMessages = validationResponse.Errors.Select(s => s.ErrorMessage).ToList();

                return new ActionResult<TResponse>().AddErrors(errorMessages);
            }

            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, ActionResult<TResponse>>>()
                 ?? throw new NullReferenceException("Invalid request handler, Register request handler");

            var response = await handler.Handle(request, cancellationToken);

            return response;
        }

        private readonly IServiceProvider serviceProvider;
    }
}
