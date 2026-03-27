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

public class For_VertegenwoordigerWerdGewijzigd
{
    private readonly BeheerVerenigingHistoriekDocument _doc;
    private readonly int _vertegenwoordigerId;
    private readonly int _nietGeanonimiseerdeVertegenwoordigerId;

    public For_VertegenwoordigerWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        _vertegenwoordigerId = fixture.Create<int>();
        _nietGeanonimiseerdeVertegenwoordigerId = fixture.Create<int>();

        _doc = BeheerVerenigingHistoriekProjectorTestHelper
            .Create(vzer)
            .ApplyVertegenwoordigerWerdToegevoegd(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdToegevoegd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerWerdGewijzigd(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdGewijzigd(_vertegenwoordigerId)
            .ApplyVertegenwoordigerWerdGewijzigd(_nietGeanonimiseerdeVertegenwoordigerId)
            .ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdToegevoegdGebeurtenis_Is_Anonymised()
    {
        var gebeurtenis = _doc.Gebeurtenissen.GetVertegenwoordigerToegevoegdGebeurtenis(_vertegenwoordigerId);

        gebeurtenis.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegd);

        ((VertegenwoordigerData)gebeurtenis.Data).ShouldBeAnonymised(_vertegenwoordigerId);
    }

    [Fact]
    public void Then_VertegenwoordigerWerdGewijzigdGebeurtenissen_Are_Anonymised() =>
        _doc
            .Gebeurtenissen.GetVertegenwoordigerGewijzigdGebeurtenissen(_vertegenwoordigerId)
            .Should()
            .AllSatisfy(x =>
            {
                x.Beschrijving.Should().Be(BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigd);
                ((VertegenwoordigerData)x.Data).ShouldBeAnonymised(_vertegenwoordigerId);
            });

    [Fact]
    public void Then_NietGeanoniseerdeVertegenwoordiger_Is_Not_Anonymised() =>
        _doc
            .Gebeurtenissen.GetVertegenwoordigerGewijzigdGebeurtenissen(_nietGeanonimiseerdeVertegenwoordigerId)
            .Should()
            .AllSatisfy(x =>
            {
                var vertegenwoordigerData = ((VertegenwoordigerData)x.Data);

                x.Beschrijving.Should()
                    .Be(
                        $"Vertegenwoordiger '{vertegenwoordigerData.Voornaam} {vertegenwoordigerData.Achternaam}' werd gewijzigd."
                    );

                vertegenwoordigerData.ShouldNotBeAnonymised();
            });
}
