﻿namespace AssociationRegistry.Framework;

using MediatR;

public record CommandEnvelope<TCommand>(TCommand Command, CommandMetadata Metadata) : IRequest<Unit> where TCommand : IRequest<Unit>;
