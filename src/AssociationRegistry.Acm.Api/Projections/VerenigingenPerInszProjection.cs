namespace AssociationRegistry.Acm.Api.Projections;

using System.Linq;
using Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection: MultiStreamAggregation<VerenigingenPerInszDocument, string>
{
    public VerenigingenPerInszProjection()
    {
        Identities<VerenigingWerdGeregistreerd>(
            geregistreerd => geregistreerd.Vertegenwoordigers
                .Select(vertegenwoordiger => vertegenwoordiger.Insz)
                .ToList());
    }

    public void Apply(VerenigingWerdGeregistreerd @event, VerenigingenPerInszDocument document)
    {
        document.Verenigingen = document.Verenigingen.Append(new Vereniging(@event.VCode, @event.Naam)).ToArray();
    }
}
