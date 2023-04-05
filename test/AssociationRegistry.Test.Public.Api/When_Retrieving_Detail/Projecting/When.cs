namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Framework;
using Framework.Helpers;

public class When<TEvent> where TEvent : notnull
{
    private readonly Fixture _fixture;
    private readonly TestEvent<TEvent> _testEvent;

    private When(TEvent @event)
    {
        _fixture = new Fixture().CustomizeAll();
        var event1 = _fixture.Create<TestEvent<TEvent>>();
        _testEvent = event1;
        _testEvent.Data = @event;
    }

    public PubliekVerenigingDetailDocument ToDetailProjectie(Func<PubliekVerenigingDetailDocument, PubliekVerenigingDetailDocument>? documentSetup = null)
    {
        var document = _fixture.Create<PubliekVerenigingDetailDocument>();
        documentSetup?.Invoke(document);

        var documentAfterChanges = document.Copy();
        new PubliekVerenigingDetailProjection().Apply((dynamic)_testEvent, documentAfterChanges);

        return documentAfterChanges;
    }

    internal static When<TEvent> Applying(Func<TEvent, TEvent>? eventAction = null)
    {
        var @event = new Fixture().CustomizeAll().Create<TEvent>();
        if (eventAction is not null)
            @event = eventAction(@event);

        return new When<TEvent>(@event);
    }
}
