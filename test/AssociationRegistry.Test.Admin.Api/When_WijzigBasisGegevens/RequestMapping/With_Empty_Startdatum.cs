namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using AutoFixture;
using FluentAssertions;
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
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigBasisgegevensRequest>();
        request.Startdatum = NullOrEmpty<DateOnly>.Empty;

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode);

        actual.Deconstruct(out var vCode, out var naam, out var korteNaam, out var korteBeschrijving, out var startdatum);

        startdatum.Should().Be(Startdatum.Leeg);
    }
}
