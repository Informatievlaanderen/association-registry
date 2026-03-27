namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Internal;

public class For_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .Create(vzer)
            .ApplyVertegenwoordigerWerdToegevoegd(_vertegenwoordigerId)
            .ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendGebeurtenis(
            _vertegenwoordigerId
        );

        gebeurtenis
            .Beschrijving.Should()
            .Be(BeheerHistoriekBeschrijvingen.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend);

        ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendGebeurtenis(
            _nietGeanonimiseerdeVertegenwoordigerId
        );

        var vertegenwoordigerData = ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data);

        gebeurtenis
            .Beschrijving.Should()
            .Be(
                $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' werd niet teruggevonden uit KSZ en werd verwijderd."
            );

        vertegenwoordigerData.ShouldNotBeAnonymised();
    }
}
