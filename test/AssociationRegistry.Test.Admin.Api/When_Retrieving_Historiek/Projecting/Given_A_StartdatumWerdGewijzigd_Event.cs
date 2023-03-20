namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
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
public class Given_A_StartdatumWerdGewijzigd_Event
{
    private readonly TestEvent<StartdatumWerdGewijzigd> _startdatumWerdGewijzigd;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    public Given_A_StartdatumWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        var beheerVerenigingHistoriekProjection = new BeheerVerenigingHistoriekProjection();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        _startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();

        _documentAfterChanges = _document.Copy();
        beheerVerenigingHistoriekProjection.Apply(_startdatumWerdGewijzigd, _documentAfterChanges);
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
                    $"Startdatum vereniging werd gewijzigd naar '{_startdatumWerdGewijzigd.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {_startdatumWerdGewijzigd.Initiator} op datum {_startdatumWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()}",
                        _startdatumWerdGewijzigd.Initiator,
                        _startdatumWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()
                )).ToList(),
                Metadata = new Metadata(_startdatumWerdGewijzigd.Sequence, _startdatumWerdGewijzigd.Version),
            }
        );
    }
}
