namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
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
        actual.IsDubbelVan.Should().BeEquivalentTo(vCodeB);
    }
}
