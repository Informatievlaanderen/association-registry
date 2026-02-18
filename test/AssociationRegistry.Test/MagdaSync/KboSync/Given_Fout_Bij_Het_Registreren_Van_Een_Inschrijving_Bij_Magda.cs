namespace AssociationRegistry.Test.MagdaSync.KboSync;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using KboMutations.SyncLambda.Exceptions;
using Xunit;

public class Given_Fout_Bij_Het_Registreren_Van_Een_Inschrijving_Bij_Magda
{
    [Fact]
    public async ValueTask Then_ThrowsRegistreerInschrijvingKonNietVoltooidWorden()
    {
        var exception = await Assert.ThrowsAsync<KboSyncException>(() =>
            new SyncKboCommandHandlerBuilder()
                .MetBestaandeVereniging()
                .MetGeldigeVerenigingVolgensMagda()
                .MetVerenigingUitVerenigingsregister()
                .MetFoutBijHetRegistrerenVanEenInschrijvingBijMagda()
                .Handle()
        );

        exception.InnerException.Should().BeOfType(typeof(RegistreerInschrijvingKonNietVoltooidWorden));
    }
}
