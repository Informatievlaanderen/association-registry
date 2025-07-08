namespace AssociationRegistry.Public.ProjectionHost.Projections.Sequence;

using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Marten.Internal.Sessions;
using Schema.Sequence;
using IEvent = Events.IEvent;

public class PubliekVerenigingSequenceProjection : SingleStreamProjection<PubliekVerenigingSequenceDocument, string>
{
    public PubliekVerenigingSequenceProjection()
    {
        Options.DeleteViewTypeOnTeardown<PubliekVerenigingSequenceDocument>();

        // CreateEvent<IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>(e =>
        // {
        //     return new PubliekVerenigingSequenceDocument
        //     {
        //         Sequence = e.Sequence,
        //         VCode = e.Data.VCode,
        //         Version = (int)e.Version,
        //     };
        // });
        // this.Apply<IEvent<FeitelijkeVerenigingWerdGeregistreerd>>((e, doc) => Update(e, doc));
    }

    public PubliekVerenigingSequenceDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> e)
    {
        return new PubliekVerenigingSequenceDocument
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };
    }

    public PubliekVerenigingSequenceDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> e)
    {
        return new PubliekVerenigingSequenceDocument
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };
    }

    public PubliekVerenigingSequenceDocument Create(IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> e)
    {
        return new PubliekVerenigingSequenceDocument
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };
    }

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> e,
        PubliekVerenigingSequenceDocument document
        )
    {
        return new PubliekVerenigingSequenceDocument
        {
            Sequence = e.Sequence,
            VCode = e.StreamKey,
            Version = (int)e.Version,
        };
    }

    // public void Apply(JasperFx.Events.IEvent e, PubliekVerenigingSequenceDocument document)
    // {
    //     document.Sequence = e.Sequence;
    //     document.VCode = e.StreamKey;
    //     document.Version = (int)e.Version;
    // }
}
