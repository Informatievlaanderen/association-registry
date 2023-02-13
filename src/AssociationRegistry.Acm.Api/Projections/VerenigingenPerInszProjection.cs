namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection : MultiStreamAggregation<VerenigingenPerInszDocument, string>
{
    public VerenigingenPerInszProjection()
    {
        Identities<VerenigingWerdGeregistreerd>(
            geregistreerd => geregistreerd.Vertegenwoordigers
                .Select(vertegenwoordiger => vertegenwoordiger.Insz)
                .ToList());
        CustomGrouping(new NaamWerdGewijzigdGrouper());
    }

    public void Apply(VerenigingWerdGeregistreerd @event, VerenigingenPerInszDocument document)
    {
        document.Verenigingen = document.Verenigingen.Append(new Vereniging(@event.VCode, @event.Naam)).ToArray();
    }

    public void Apply(NaamWerdGewijzigd @event, VerenigingenPerInszDocument document)
    {
        document.Verenigingen.Single(v => v.VCode == @event.VCode).Naam = @event.Naam;
    }
}

public class NaamWerdGewijzigdGrouper : IAggregateGrouper<string>
{
    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<string> grouping)
    {
        var naamWerdGewijzigdEvents = events
            .OfType<IEvent<NaamWerdGewijzigd>>()
            .ToList();

        if (!naamWerdGewijzigdEvents.Any())
        {
            return;
        }

        foreach (var @event in naamWerdGewijzigdEvents)
        {
            var documents = await session.Query<VerenigingenPerInszDocument>()
                .Where(doc =>
                    doc.Verenigingen
                        .Select(v => v.VCode)
                        .Contains(@event.Data.VCode))
                .ToListAsync();

            foreach (var document in documents)
            {
                grouping.AddEvent(document.Insz, @event);
            }
        }
    }
}
