namespace AssociationRegistry.Test.MagdaSync.KboSync;

using KboMutations.SyncLambda.Exceptions;
using Xunit;

public class Given_Fout_Bij_Laden_Vereniging_In_Verenigingsregister
{
    [Fact]
    public async ValueTask Then_ThrowsException()
    {
        await Assert.ThrowsAsync<KboSyncException>(() =>
            new SyncKboCommandHandlerBuilder()
                .MetBestaandeVereniging()
                .MetGeldigeVerenigingVolgensMagda()
                .MetFoutBijLadenVereniging()
                .Handle()
        );
    }
}
