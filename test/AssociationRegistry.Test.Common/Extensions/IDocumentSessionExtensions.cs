namespace AssociationRegistry.Test.Common.Extensions;

using Events;
using Marten;

public static class DocumentSessionExtensions
{
    public static T? SingleOrDefaultFromStream<T>(this IDocumentSession session, string vCode) where T : IEvent
    {
        var stream = session.Events.FetchStreamAsync(vCode).GetAwaiter().GetResult();
        var @event = stream.SingleOrDefault(e => e.Data.GetType().Name == typeof(T).Name);

        return @event is not null
            ? (T)@event.Data
            : default;
    }
}
