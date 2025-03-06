namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Framework;
using NodaTime;
using System;

public class CommandMetadataProviderStub : ICommandMetadataProvider
{
    public string Initiator { get; set; } = null!;
    public Instant Tijdstip { get; set; }
    public Guid CorrelationId { get; set; }
    public long? ExpectedVersion { get; set; }

    public CommandMetadata GetMetadata(long? expectedVersion = null)
        => new(
            Initiator,
            Tijdstip,
            CorrelationId,
            expectedVersion ?? ExpectedVersion);
}
