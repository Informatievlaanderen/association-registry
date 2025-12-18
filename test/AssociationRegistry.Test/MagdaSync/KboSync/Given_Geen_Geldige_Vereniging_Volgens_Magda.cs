namespace AssociationRegistry.Test.MagdaSync.KboSync;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_Geen_Geldige_Vereniging_Volgens_Magda
{
    [Fact]
    public async ValueTask Then_Throws_GeenGeldigeVerenigingInKbo()
    {
        await Assert
           .ThrowsAsync<GeenGeldigeVerenigingInKbo>(() => new SyncKboCommandHandlerBuilder()
                                                         .MetBestaandeVereniging()
                                                         .MetGeenGeldigeVerenigingVolgensMagda()
                                                         .Handle());
    }
}
