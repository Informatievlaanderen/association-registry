namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Helpers;

public class WhenApplying<TEvent> where TEvent : notnull
{
    internal readonly TestEvent<TEvent> Event;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    protected WhenApplying() : this(new Fixture().CustomizeAll().Create<TEvent>())
    {
    }

    private WhenApplying(TEvent @event)
    {
        var fixture = new Fixture().CustomizeAll();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        Event = fixture.Create<TestEvent<TEvent>>();
        Event.Data = @event;

        _documentAfterChanges = _document.Copy();
        new BeheerVerenigingHistoriekProjection().Apply((dynamic)Event, _documentAfterChanges);
    }

    internal static WhenApplying<TEvent> ToHistoriekProjectie(TEvent @event)
        => new(@event);

    internal void AppendsTheCorrectGebeurtenissen(params string[] appendedGebeurtenissen)
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
