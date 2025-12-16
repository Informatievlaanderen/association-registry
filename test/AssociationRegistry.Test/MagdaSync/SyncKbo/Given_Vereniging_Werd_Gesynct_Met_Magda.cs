namespace AssociationRegistry.Test.KboSync;

using FluentAssertions;
using Xunit;

public class Given_Vereniging_Werd_Gesynct_Met_Magda
{
    [Fact]
    public async ValueTask Then_Completes_Successfully()
    {
        var act = async () => await new SyncKboCommandHandlerBuilder()
                          .MetBestaandeVereniging()
                          .MetGeldigeVerenigingVolgensMagda()
                          .MetVerenigingUitVerenigingsregister()
                          .MetSuccesBijHetRegistrerenVanEenInschrijvingBijMagda()
                          .MetSuccesvolOpgeslagenVereniging()
                          .Handle();

        await act.Should().NotThrowAsync();
    }
}
