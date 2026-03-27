namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Xunit;

public class For_VertegenwoordigerWerdVerwijderd
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_VertegenwoordigerWerdVerwijderd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .Create(vzer)
            .ApplyVertegenwoordigerWerdToegevoegd(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerWerdVerwijderd(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdVerwijderd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdVerwijderdGebeurtenis_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerVerwijderdGebeurtenis(_vertegenwoordigerId);

        gebeurtenis.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdVerwijderd);

        ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerVerwijderdGebeurtenis(
            _nietGeanonimiseerdeVertegenwoordigerId
        );

        var vertegenwoordigerData = ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data);

        gebeurtenis
            .Beschrijving.Should()
            .Be(
                $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' werd verwijderd."
            );

        vertegenwoordigerData.ShouldNotBeAnonymised();
    }
}
