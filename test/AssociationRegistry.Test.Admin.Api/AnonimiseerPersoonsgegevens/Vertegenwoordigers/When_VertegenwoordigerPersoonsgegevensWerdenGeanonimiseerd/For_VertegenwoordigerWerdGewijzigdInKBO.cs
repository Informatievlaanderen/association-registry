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

public class For_VertegenwoordigerWerdGewijzigdInKBO
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_VertegenwoordigerWerdGewijzigdInKBO()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var kbo = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .CreateKBO(kbo)
            .ApplyVertegenwoordigerWerdToegevoegdVanuitKBO(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegdVanuitKBO(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerWerdGewijzigdInKBO(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdGewijzigdInKBO(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdGewijzigdInKBOGebeurtenis_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerWerdToegevoegdVanuitKBOGebeurtenis(
            _vertegenwoordigerId
        );

        gebeurtenis.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegdVanuitKBO);

        ((KBOVertegenwoordigerData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdGewijzigdInKBOGebeurtenissen_Are_Anonymised() =>
        _doc
            .Gebeurtenissen.GetVertegenwoordigerWerdGewijzigdInKBOGebeurtenissen(_vertegenwoordigerId)
            .Should()
            .AllSatisfy(x =>
            {
                x.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigdInKBO);
                ((KBOVertegenwoordigerData)x.Data).ShouldBeAnonymised(_vertegenwoordigerId);
            });

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised() =>
        _doc
            .Gebeurtenissen.GetVertegenwoordigerWerdGewijzigdInKBOGebeurtenissen(
                _nietGeanonimiseerdeVertegenwoordigerId
            )
            .Should()
            .AllSatisfy(x =>
            {
                var vertegenwoordigerData = ((KBOVertegenwoordigerData)x.Data);

                x.Beschrijving.Should()
                    .Be(
                        $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' werd gewijzigd in KBO."
                    );

                vertegenwoordigerData.ShouldNotBeAnonymised();
            });
}
