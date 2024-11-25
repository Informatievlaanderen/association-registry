namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework;
using Primitives;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class With_Empty_Startdatum
{
    [Fact]
    public void Then_We_Get_An_Empty_Startdatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigBasisgegevensRequest>();
        request.Startdatum = NullOrEmpty<DateOnly>.Empty;

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode);

        actual.Startdatum.Should().Be(NullOrEmpty<Datum>.Empty);
    }
}
