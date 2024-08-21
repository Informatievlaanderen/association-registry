namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_A_StopVerenigingCommand
{
    [Fact]
    public void Then_We_Get_A_StopVerenigingCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<StopVerenigingRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode);

        actual.Deconstruct(
            out var vCode,
            out var einddatum);

        vCode.Should().Be(actualVCode);
        einddatum.Should().Be(Datum.CreateOptional(request.Einddatum));
    }
}
