namespace AssociationRegistry.Test.MagdaSync.KboSync;

using Xunit;

public class Given_Fout_Bij_Laden_Vereniging_In_Verenigingsregister
{
    [Fact]
    public async ValueTask Then_ThrowsException()
    {
        await Assert
           .ThrowsAsync<Exception>(() => new SyncKboCommandHandlerBuilder()
                                        .MetBestaandeVereniging()
                                        .MetGeldigeVerenigingVolgensMagda()
                                        .MetFoutBijLadenVereniging()
                                        .Handle());
    }
}
