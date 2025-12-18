namespace AssociationRegistry.Test.MagdaSync.KboSync;

using FluentAssertions;
using Xunit;

public class Given_Vereniging_Niet_Gekend_In_Verenigingsregister
{
    [Fact]
    public async ValueTask Then_Returns_Null()
    {
        var actual = await new SyncKboCommandHandlerBuilder()
                 .MetNietBestaandeVereniging()
                 .Handle();

        actual.Should().BeNull();
    }
}
