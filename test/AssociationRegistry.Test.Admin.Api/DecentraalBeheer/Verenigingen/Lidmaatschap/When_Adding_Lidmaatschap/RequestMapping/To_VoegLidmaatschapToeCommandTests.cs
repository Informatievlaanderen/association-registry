namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_VoegLidmaatschapToeCommandTests
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var command = request.ToCommand(vCode, andereVerenigingNaam);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.AndereVerenigingNaam.Should().Be(andereVerenigingNaam);
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(new GeldigVan(request.Van));
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(new GeldigTot(request.Tot));
        command.Lidmaatschap.Identificatie.Should().Be(LidmaatschapIdentificatie.Hydrate(request.Identificatie));
        command.Lidmaatschap.Beschrijving.Should().Be(LidmaatschapBeschrijving.Hydrate(request.Beschrijving));
    }

    [Fact]
    public void With_Van_Null_Then_Van_Should_Be_Infinite()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        request.Van = null;

        request.Identificatie = null;

        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var command = request.ToCommand(vCode, andereVerenigingNaam);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.AndereVerenigingNaam.Should().Be(andereVerenigingNaam);
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(GeldigVan.Infinite);
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(new GeldigTot(request.Tot));
        command.Lidmaatschap.Identificatie.Should().Be(LidmaatschapIdentificatie.Hydrate(string.Empty));
        command.Lidmaatschap.Beschrijving.Should().Be(LidmaatschapBeschrijving.Hydrate(request.Beschrijving));
    }

    [Fact]
    public void With_Tot_Null_Then_Tot_Should_Be_Infinite()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        request.Tot = null;

        request.Identificatie = null;

        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var command = request.ToCommand(vCode, andereVerenigingNaam);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.AndereVerenigingNaam.Should().Be(andereVerenigingNaam);
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(new GeldigVan(request.Van));
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(GeldigTot.Infinite);
        command.Lidmaatschap.Identificatie.Should().Be(LidmaatschapIdentificatie.Hydrate(string.Empty));
        command.Lidmaatschap.Beschrijving.Should().Be(LidmaatschapBeschrijving.Hydrate(request.Beschrijving));
    }

    [Fact]
    public void With_Identificatie_Null_Then_Identificatie_Should_Be_Empty()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        request.Identificatie = null;

        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var command = request.ToCommand(vCode, andereVerenigingNaam);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.AndereVerenigingNaam.Should().Be(andereVerenigingNaam);
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(new GeldigVan(request.Van));
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(new GeldigTot(request.Tot));
        command.Lidmaatschap.Identificatie.Should().Be(LidmaatschapIdentificatie.Hydrate(string.Empty));
        command.Lidmaatschap.Beschrijving.Should().Be(LidmaatschapBeschrijving.Hydrate(request.Beschrijving));
    }

    [Fact]
    public void With_Beschrijving_Null_Then_Beschrijving_Should_Be_Empty()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        request.Beschrijving = null;

        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var command = request.ToCommand(vCode, andereVerenigingNaam);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.AndereVerenigingNaam.Should().Be(andereVerenigingNaam);
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(new GeldigVan(request.Van));
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(new GeldigTot(request.Tot));
        command.Lidmaatschap.Identificatie.Should().Be(LidmaatschapIdentificatie.Hydrate(request.Identificatie));
        command.Lidmaatschap.Beschrijving.Should().Be(LidmaatschapBeschrijving.Hydrate(string.Empty));
    }
}
