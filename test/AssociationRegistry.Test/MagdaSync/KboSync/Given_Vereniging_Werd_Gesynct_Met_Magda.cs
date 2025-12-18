namespace AssociationRegistry.Test.MagdaSync.KboSync;

using FluentAssertions;
using Xunit;

public class Given_Vereniging_Werd_Gesynct_Met_Magda
{
    [Fact]
    public async ValueTask Then_Returns_CommandResult()
    {
        var actual = await new SyncKboCommandHandlerBuilder()
                          .MetBestaandeVereniging()
                          .MetGeldigeVerenigingVolgensMagda()
                          .MetVerenigingUitVerenigingsregister()
                          .MetSuccesBijHetRegistrerenVanEenInschrijvingBijMagda()
                          .MetSuccesvolOpgeslagenVereniging()
                          .Handle();

        actual.Should().NotBeNull();
    }
}
