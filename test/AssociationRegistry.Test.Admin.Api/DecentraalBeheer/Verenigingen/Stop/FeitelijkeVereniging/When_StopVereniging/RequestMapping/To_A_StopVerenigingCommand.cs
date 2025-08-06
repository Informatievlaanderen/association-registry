namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Stop.FeitelijkeVereniging.When_StopVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

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
