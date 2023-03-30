namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using Framework;
using Framework.Helpers;
using AutoFixture;

public class WhenApplying<TEvent> where TEvent : notnull
{
    public readonly BeheerVerenigingDetailDocument DocumentAfterChanges;

    private WhenApplying(TEvent @event)
    {
        var fixture = new Fixture().CustomizeAll();
        var document = fixture.Create <BeheerVerenigingDetailDocument>();
        var event1 = fixture.Create<TestEvent<TEvent>>();
        event1.Data = @event;

        DocumentAfterChanges = document.Copy();
        new BeheerVerenigingDetailProjection().Apply((dynamic)event1, DocumentAfterChanges);
    }

    internal static WhenApplying<TEvent> ToDetailProjectie(Func<TEvent, TEvent>? eventAction = null)
    {
        var @event = new Fixture().CustomizeAll().Create<TEvent>();
        if (eventAction is not null)
            @event = eventAction(@event);

        return new WhenApplying<TEvent>(@event);
    }
}
