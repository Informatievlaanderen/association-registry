namespace AssociationRegistry.Test.KboSync;

using Xunit;

public class Given_Fout_Bij_Ophalen_Vereniging_Bij_Magda
{
    [Fact]
    public async Task Then_Throws_Exception()
    {
        await Assert
           .ThrowsAsync<Exception>(() => new SyncKboCommandHandlerBuilder()
                                        .MetBestaandeVereniging()
                                        .MetFoutBijVerenigingOphalenBijMagda()
                                        .Handle());
    }
}
