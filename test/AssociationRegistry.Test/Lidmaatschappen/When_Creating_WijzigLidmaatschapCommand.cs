namespace AssociationRegistry.Test.Lidmaatschappen;

using Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using AssociationRegistry.Acties.WijzigLidmaatschap;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class When_Creating_WijzigLidmaatschapCommand
{
    private readonly Fixture _fixture;
    private readonly VCode _someVCode;
    private readonly int _someLidmaatschapId;

    public When_Creating_WijzigLidmaatschapCommand()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _someVCode = _fixture.Create<VCode>();
        _someLidmaatschapId = _fixture.Create<int>();
    }

    [Fact]
    public void Given_All_Nulls()
    {
        var request = new WijzigLidmaatschapRequest()
        {
            Van = NullOrEmpty<DateOnly>.Null,
            Tot = NullOrEmpty<DateOnly>.Null,
            Beschrijving = null,
            Identificatie = null
        };

        var actual = request.ToCommand(
            _someVCode,
            _someLidmaatschapId);

        actual.Lidmaatschap.Should().BeEquivalentTo(
            new WijzigLidmaatschapCommand.TeWijzigenLidmaatschap(
                new LidmaatschapId(_someLidmaatschapId),
                null,
                null,
                null,
                null
            ));

    }

    [Fact]
    public void Given_All_Empties()
    {
        var request = new WijzigLidmaatschapRequest()
        {
            Van = NullOrEmpty<DateOnly>.Empty,
            Tot = NullOrEmpty<DateOnly>.Empty,
            Beschrijving = string.Empty,
            Identificatie = string.Empty,
        };

        var actual = request.ToCommand(
            _someVCode,
            _someLidmaatschapId);

        actual.Lidmaatschap.Should().BeEquivalentTo(
            new WijzigLidmaatschapCommand.TeWijzigenLidmaatschap(
                new LidmaatschapId(_someLidmaatschapId),
                GeldigVan.Infinite,
                GeldigTot.Infinite,
                LidmaatschapIdentificatie.Create(""),
                LidmaatschapBeschrijving.Create("")
                ));
    }
}
