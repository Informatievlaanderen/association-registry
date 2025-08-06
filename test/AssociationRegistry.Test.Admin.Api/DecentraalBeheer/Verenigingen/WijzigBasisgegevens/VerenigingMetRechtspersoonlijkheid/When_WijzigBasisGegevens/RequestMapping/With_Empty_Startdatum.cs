namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class With_Empty_Startdatum
{
    [Fact]
    public void Then_We_Get_An_Empty_Startdatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var werkingsgebiedenService = new WerkingsgebiedenServiceMock();

        var request = fixture.Create<WijzigBasisgegevensRequest>();
        request.Startdatum = NullOrEmpty<DateOnly>.Empty;

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode, werkingsgebiedenService);

        actual.Startdatum.Should().Be(NullOrEmpty<Datum>.Empty);
    }
}
