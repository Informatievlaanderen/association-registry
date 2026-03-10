namespace AssociationRegistry.Test.MagdaSync.KboSync;

using CommandHandling.MagdaSync.Exceptions;
using Xunit;

public class Given_Fout_Bij_Ophalen_Vereniging_Bij_Magda
{
    [Fact]
    public async ValueTask Then_Throws_Exception()
    {
        await Assert.ThrowsAsync<KboSyncException>(() =>
            new SyncKboCommandHandlerBuilder().MetBestaandeVereniging().MetFoutBijVerenigingOphalenBijMagda().Handle()
        );
    }
}
