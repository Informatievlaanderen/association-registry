namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using Framework;
using NodaTime;
using System;

internal class CommandMetadataProvider : ICommandMetadataProvider
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
        => new(
            _initiatorProvider.Value,
            SystemClock.Instance.GetCurrentInstant(),
            Guid.Parse(_correlationIdProvider.CorrelationId),
            expectedVersion);
}
