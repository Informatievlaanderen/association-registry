namespace AssociationRegistry.Framework;

using MediatR;
using Vereniging;

public record CommandEnvelope<TCommand>(TCommand Command, CommandMetadata Metadata) : IRequest<RegistratieResult> where TCommand : IRequest<RegistratieResult>;
