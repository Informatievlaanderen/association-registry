namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Xunit;

public class For_VertegenwoordigerWerdVerwijderdUitKBO
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_VertegenwoordigerWerdVerwijderdUitKBO()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var kbo = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .CreateKBO(kbo)
            .ApplyVertegenwoordigerWerdToegevoegdVanuitKBO(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegdVanuitKBO(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerWerdVerwijderdUitKBO(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdVerwijderdUitKBO(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdVerwijderdGebeurtenis_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerVerwijderdUitKBOGebeurtenis(_vertegenwoordigerId);

        gebeurtenis.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdVerwijderdUitKBO);

        ((KBOVertegenwoordigerData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerVerwijderdUitKBOGebeurtenis(
            _nietGeanonimiseerdeVertegenwoordigerId
        );

        var vertegenwoordigerData = ((KBOVertegenwoordigerData)gebeurtenis.Data);

        gebeurtenis
            .Beschrijving.Should()
            .Be(
                $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' werd verwijderd uit KBO."
            );

        vertegenwoordigerData.ShouldNotBeAnonymised();
    }
}
