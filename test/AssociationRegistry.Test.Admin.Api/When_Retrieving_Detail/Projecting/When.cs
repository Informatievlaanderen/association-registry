namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using Framework;
using Framework.Helpers;
using AutoFixture;

public class When<TEvent> where TEvent : notnull
{
    private readonly Fixture _fixture;
    private readonly TestEvent<TEvent> _event;

    private When(TEvent @event)
    {
        _fixture = new Fixture().CustomizeAll();
        _event = _fixture.Create<TestEvent<TEvent>>();
        _event.Data = @event;
    }

    public BeheerVerenigingDetailDocument ToDetailProjectie(Func<BeheerVerenigingDetailDocument, BeheerVerenigingDetailDocument>? documentSetup = null)
    {
        var document = _fixture.Create<BeheerVerenigingDetailDocument>();
        documentSetup?.Invoke(document);
        var copy = document.Copy();
        new BeheerVerenigingDetailProjection().Apply((dynamic)_event, copy);
        return copy;
    }

    internal static When<TEvent> Applying(Func<TEvent, TEvent>? eventAction = null)
    {
        var @event = new Fixture().CustomizeAll().Create<TEvent>();
        if (eventAction is not null)
            @event = eventAction(@event);

        return new When<TEvent>(@event);
    }
}
