namespace AssociationRegistry.Test.KboSync;

using FluentAssertions;
using Xunit;

public class Given_Vereniging_Niet_Gekend_In_Verenigingsregister
{
    [Fact]
    public async ValueTask Then_Throws_Exception()
    {
        var act = async () => await new SyncKboCommandHandlerBuilder()
                 .MetNietBestaandeVereniging()
                 .Handle();

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("KboNummer not found");
    }
}
