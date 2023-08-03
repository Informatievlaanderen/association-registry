namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using System;
using Framework;
using NodaTime;

class CommandMetadataProvider : ICommandMetadataProvider
{
    private readonly ICorrelationIdProvider _correlationIdProvider;
    private readonly InitiatorProvider _initiatorProvider;

    public CommandMetadataProvider(
        ICorrelationIdProvider correlationIdProvider,
        InitiatorProvider initiatorProvider)
    {
        _correlationIdProvider = correlationIdProvider;
        _initiatorProvider = initiatorProvider;
    }

    public CommandMetadata GetMetadata(long? expectedVersion = null)
        => new CommandMetadata(
            _initiatorProvider.Value,
            SystemClock.Instance.GetCurrentInstant(),
            Guid.Parse(_correlationIdProvider.CorrelationId),
            expectedVersion);
}
