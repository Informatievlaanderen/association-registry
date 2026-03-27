namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class For_VzerWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd _vzer;

    public For_VzerWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        _vertegenwoordigerId = _vzer.Vertegenwoordigers.First().VertegenwoordigerId;

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .Create(_vzer)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    private VerenigingWerdGeregistreerdData GetData() =>
        _doc
            .Gebeurtenissen.Single(x =>
                x.Gebeurtenis == nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
            )
            .Data.Should()
            .BeOfType<VerenigingWerdGeregistreerdData>()
            .Subject;

    [Fact]
    public void Then_Vertegenwoordiger_Is_Anonymised() =>
        GetData()
            .Vertegenwoordigers.Single(v => v.VertegenwoordigerId == _vertegenwoordigerId)
            .ShouldBeAnonymised(_vertegenwoordigerId);

    [Fact]
    public void Then_Beschrijving_Is_Still_The_Same() =>
        _doc
            .Gebeurtenissen.Single(x =>
                x.Gebeurtenis == nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
            )
            .Beschrijving.Should()
            .Contain($"Vereniging zonder eigen rechtspersoonlijkheid werd geregistreerd met naam '{_vzer.Naam}'.");

    [Fact]
    public void Then_Other_Vertegenwoordigers_Are_Not_Anonymised() =>
        GetData()
            .Vertegenwoordigers.Where(v => v.VertegenwoordigerId != _vertegenwoordigerId)
            .Should()
            .AllSatisfy(v => v.ShouldNotBeAnonymised());
}
