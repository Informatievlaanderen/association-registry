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

public class For_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .Create(vzer)
            .ApplyVertegenwoordigerWerdToegevoegd(_vertegenwoordigerId)
            .ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetKszSyncHeeftVertegenwoordigerAangeduidAlsOverledenGebeurtenis(
            _vertegenwoordigerId
        );

        gebeurtenis
            .Beschrijving.Should()
            .Be(BeheerHistoriekBeschrijvingen.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden);

        ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetKszSyncHeeftVertegenwoordigerAangeduidAlsOverledenGebeurtenis(
            _nietGeanonimiseerdeVertegenwoordigerId
        );

        var vertegenwoordigerData = ((VertegenwoordigerWerdVerwijderdData)gebeurtenis.Data);

        gebeurtenis
            .Beschrijving.Should()
            .Be(
                $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' is overleden volgens KSZ en werd verwijderd."
            );

        vertegenwoordigerData.ShouldNotBeAnonymised();
    }
}
