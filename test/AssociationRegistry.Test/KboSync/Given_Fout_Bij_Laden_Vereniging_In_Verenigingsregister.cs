namespace AssociationRegistry.Test.KboSync;

using Xunit;

public class Given_Fout_Bij_Laden_Vereniging_In_Verenigingsregister
{
    [Fact]
    public async Task Then_ThrowsException()
    {
        await Assert
           .ThrowsAsync<Exception>(() => new SyncKboCommandHandlerBuilder()
                                        .MetBestaandeVereniging()
                                        .MetGeldigeVerenigingVolgensMagda()
                                        .MetFoutBijLadenVereniging()
                                        .Handle());
    }
}
