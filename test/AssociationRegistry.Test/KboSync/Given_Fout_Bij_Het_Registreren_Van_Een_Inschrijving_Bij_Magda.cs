namespace AssociationRegistry.Test.KboSync;

using Vereniging.Exceptions;
using Xunit;

public class Given_Fout_Bij_Het_Registreren_Van_Een_Inschrijving_Bij_Magda
{
    [Fact]
    public async ValueTask Then_ThrowsRegistreerInschrijvingKonNietVoltooidWorden()
    {
        await Assert
           .ThrowsAsync<RegistreerInschrijvingKonNietVoltooidWorden>(() => new SyncKboCommandHandlerBuilder()
                                                                          .MetBestaandeVereniging()
                                                                          .MetGeldigeVerenigingVolgensMagda()
                                                                          .MetVerenigingUitVerenigingsregister()
                                                                          .MetFoutBijHetRegistrerenVanEenInschrijvingBijMagda()
                                                                          .Handle());
    }
}
