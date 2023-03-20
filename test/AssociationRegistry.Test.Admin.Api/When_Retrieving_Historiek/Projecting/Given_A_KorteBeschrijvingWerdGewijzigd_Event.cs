namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Helpers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_KorteBeschrijvingWerdGewijzigd_Event
{
    private readonly TestEvent<KorteBeschrijvingWerdGewijzigd> _korteBeschrijvingWerdGewijzigd;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    public Given_A_KorteBeschrijvingWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        _korteBeschrijvingWerdGewijzigd = fixture.Create<TestEvent<KorteBeschrijvingWerdGewijzigd>>();

        _documentAfterChanges = _document.Copy();
        beheerVerenigingHistoriekProjection.Apply(_korteBeschrijvingWerdGewijzigd, _documentAfterChanges);
    }

    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen.Append(new BeheerVerenigingHistoriekGebeurtenis
                (
                    $"Korte beschrijving vereniging werd gewijzigd naar '{_korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}' door {_korteBeschrijvingWerdGewijzigd.Initiator} op datum {_korteBeschrijvingWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()}",
                        _korteBeschrijvingWerdGewijzigd.Initiator,
                        _korteBeschrijvingWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()
                )).ToList(),
                Metadata = new Metadata(_korteBeschrijvingWerdGewijzigd.Sequence, _korteBeschrijvingWerdGewijzigd.Version),
            }
        );
    }
}
