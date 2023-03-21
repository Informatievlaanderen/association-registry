namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Helpers;
using Xunit.Categories;

[UnitTest]
public abstract class GivenAnEventTestBase<TEvent> where TEvent : notnull
{
    protected readonly TestEvent<TEvent> Event;
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly BeheerVerenigingHistoriekDocument _documentAfterChanges;

    protected GivenAnEventTestBase()
    {
        var fixture = new Fixture().CustomizeAll();
        _document = fixture.Create<BeheerVerenigingHistoriekDocument>();
        Event = fixture.Create<TestEvent<TEvent>>();

        _documentAfterChanges = _document.Copy();
    }

    protected void AppendsTheCorrectGebeurtenissen(params string[] appendedGebeurtenissen)
    {
        new BeheerVerenigingHistoriekProjection().Apply((dynamic)Event, _documentAfterChanges);

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
