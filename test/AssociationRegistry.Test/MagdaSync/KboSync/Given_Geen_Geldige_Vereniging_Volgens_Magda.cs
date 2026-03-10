namespace AssociationRegistry.Test.MagdaSync.KboSync;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using CommandHandling.MagdaSync.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Geen_Geldige_Vereniging_Volgens_Magda
{
    [Fact]
    public async ValueTask Then_Throws_GeenGeldigeVerenigingInKbo()
    {
        var exception = await Assert.ThrowsAsync<KboSyncException>(() =>
            new SyncKboCommandHandlerBuilder().MetBestaandeVereniging().MetGeenGeldigeVerenigingVolgensMagda().Handle()
        );

        exception.InnerException.Should().BeOfType(typeof(GeenGeldigeVerenigingInKbo));
    }
}
