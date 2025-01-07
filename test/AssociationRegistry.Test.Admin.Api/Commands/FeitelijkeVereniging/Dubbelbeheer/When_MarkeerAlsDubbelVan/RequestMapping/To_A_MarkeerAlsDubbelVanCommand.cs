namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.Dubbelbeheer.When_MarkeerAlsDubbelVan.RequestMapping;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class To_A_MarkeerAlsDubbelVanCommand
{
    [Fact]
    public void Then_We_Get_A_MarkeerAlsDubbelVanCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCodeA = fixture.Create<VCode>();
        var vCodeB = fixture.Create<VCode>();

        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = vCodeB.ToString(),
        };

        var actual = request.ToCommand(vCodeA);

        actual.VCode.Should().BeEquivalentTo(vCodeA);
        actual.VCodeAuthentiekeVereniging.Should().BeEquivalentTo(vCodeB);
    }
}
