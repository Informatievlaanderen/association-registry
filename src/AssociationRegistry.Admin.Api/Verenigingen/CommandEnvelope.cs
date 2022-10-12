namespace AssociationRegistry.Admin.Api.Verenigingen;

using MediatR;

public record CommandEnvelope<TCommand>(TCommand Command) : IRequest<Unit> where TCommand:IRequest<Unit>;
