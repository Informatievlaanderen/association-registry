namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Framework;
using Framework.Helpers;

public class WhenApplying<TEvent> where TEvent : notnull
{
    public readonly PubliekVerenigingDetailDocument DocumentAfterChanges;

    private WhenApplying(TEvent @event)
    {
        var fixture = new Fixture().CustomizeAll();
        var document = fixture.Create<PubliekVerenigingDetailDocument>();
        var event1 = fixture.Create<TestEvent<TEvent>>();
        event1.Data = @event;

        DocumentAfterChanges = document.Copy();
        new PubliekVerenigingDetailProjection().Apply((dynamic)event1, DocumentAfterChanges);
    }

    internal static WhenApplying<TEvent> ToDetailProjectie(Func<TEvent, TEvent>? eventAction = null)
    {
        var @event = new Fixture().CustomizeAll().Create<TEvent>();
        if (eventAction is not null)
            @event = eventAction(@event);

        return new WhenApplying<TEvent>(@event);
    }
}
