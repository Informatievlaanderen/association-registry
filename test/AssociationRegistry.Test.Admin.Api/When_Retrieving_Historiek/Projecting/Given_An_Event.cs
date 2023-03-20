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
public abstract class Given_An_Event<TEvent> where TEvent : notnull
{
    protected readonly TestEvent<TEvent> Event;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    protected Given_An_Event()
    {
        var fixture = new Fixture().CustomizeAll();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        Event = fixture.Create<TestEvent<TEvent>>();

        _documentAfterChanges = _document.Copy();

        new BeheerVerenigingHistoriekProjection().Apply((dynamic)Event, _documentAfterChanges);
    }

    protected void AppendsTheCorrectGebeurtenissen(params string[] appendedGebeurtenissen)
    {
        _documentAfterChanges.Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekDocument
            {
                VCode = _document.VCode,
                Gebeurtenissen = _document.Gebeurtenissen.Concat(
                    appendedGebeurtenissen.Select(
                        gebeurtenis => new BeheerVerenigingHistoriekGebeurtenis(
                            gebeurtenis,
                            Event.Initiator,
                            Event.Tijdstip.ToBelgianDateAndTime()))).ToList(),
                Metadata = new Metadata(Event.Sequence, Event.Version),
            }
        );
    }
}

public class Given_A_KorteNaamWerdGewijzigd_Event : Given_An_Event<KorteNaamWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte naam vereniging werd gewijzigd naar '{Event.Data.KorteNaam}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}");
}

public class Given_A_NaamWerdGewijzigd_Event : Given_An_Event<NaamWerdGewijzigd>
{
    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Naam vereniging werd gewijzigd naar '{Event.Data.Naam}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}");
}

public class Given_A_KorteBeschrijvingWerdGewijzigd_Event : Given_An_Event<KorteBeschrijvingWerdGewijzigd>
{
    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte beschrijving vereniging werd gewijzigd naar '{Event.Data.KorteBeschrijving}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}");
}

public class Given_A_StartdatumWerdGewijzigd_Event : Given_An_Event<StartdatumWerdGewijzigd>
{
    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar '{Event.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}");
}
