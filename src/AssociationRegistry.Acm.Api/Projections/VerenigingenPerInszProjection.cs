namespace AssociationRegistry.Acm.Api.Projections;

using Events;
using JasperFx.Events;
using Marten;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VerenigingenPerInszProjection : EventProjection
{
    public VerenigingenPerInszProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted VerenigingenPerInszDocument from FeitelijkeVerenigingWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd
        Options.BatchSize = 1;
        Options.MaximumHopperSize = 1;
        Options.DeleteViewTypeOnTeardown<VerenigingenPerInszDocument>();
        Options.DeleteViewTypeOnTeardown<VerenigingDocument>();
    }

    public async Task Project(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(werdGeregistreerd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(werdGeregistreerd, ops));

        ops.StoreObjects(docs);
    }

    public void Project(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(rechtsvormWerdGewijzigdInKbo, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(rechtsvormWerdGewijzigdInKbo, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(NaamWerdGewijzigd naamWerdGewijzigd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(naamWerdGewijzigd, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(naamWerdGewijzigd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(naamWerdGewijzigdInKbo, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(naamWerdGewijzigdInKbo, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdToegevoegd, ops));

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdVerwijderd, ops));
    public async Task Project(IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdVerwijderd, ops));
    public async Task Project(IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdVerwijderd, ops));

    public async Task Project(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdOvergenomenUitKbo, ops));

    public async Task Project(
        IEvent<VertegenwoordigerWerdToegevoegdVanuitKBO> @event,
        IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(@event, ops));

    public async Task Project(
        IEvent<VertegenwoordigerWerdVerwijderdUitKBO> @event,
        IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(@event, ops));

    public async Task Project(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(verenigingWerdGestopt, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(verenigingWerdGestopt, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(verenigingWerdVerwijderd, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(verenigingWerdVerwijderd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> verenigingWerdGemarkeerdAlsDubbelVan, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.AddRange(await VerenigingenPerInszProjector.Apply(verenigingWerdGemarkeerdAlsDubbelVan, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<VerenigingAanvaarddeDubbeleVereniging> verenigingAanvaarddeDubbeleVereniging, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(verenigingAanvaarddeDubbeleVereniging, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(verenigingAanvaarddeDubbeleVereniging, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> markeringDubbeleVerengingWerdGecorrigeerd,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.AddRange(await VerenigingenPerInszProjector.Apply(markeringDubbeleVerengingWerdGecorrigeerd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(@event, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(@event, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(@event, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(@event, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(
        IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event,
        IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(@event, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(@event, ops));

        ops.StoreObjects(docs);
    }


}
