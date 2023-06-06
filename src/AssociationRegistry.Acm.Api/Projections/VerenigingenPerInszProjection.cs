namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection : EventProjection
{
    public VerenigingenPerInszProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted VerenigingenPerInszDocument from FeitelijkeVerenigingWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd
        Options.BatchSize = 1;
    }

    public async Task Project(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(werdGeregistreerd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(AfdelingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(werdGeregistreerd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(NaamWerdGewijzigd naamWerdGewijzigd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(await VerenigingDocumentProjector.Apply(naamWerdGewijzigd, ops));
        docs.AddRange(await VerenigingenPerInszProjector.Apply(naamWerdGewijzigd, ops));

        ops.StoreObjects(docs);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdToegevoegd, ops));

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdVerwijderd, ops));

    private static class VerenigingenPerInszProjector
    {
        public static async Task<List<VerenigingenPerInszDocument>> Apply(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var vereniging = new Vereniging
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
            };

            foreach (var vertegenwoordiger in werdGeregistreerd.Vertegenwoordigers)
            {
                var verenigingenPerInszDocument = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordiger.Insz);
                verenigingenPerInszDocument.Verenigingen.Add(vereniging);
                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<List<VerenigingenPerInszDocument>> Apply(AfdelingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var vereniging = new Vereniging
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
            };

            foreach (var vertegenwoordiger in werdGeregistreerd.Vertegenwoordigers)
            {
                var verenigingenPerInszDocument = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordiger.Insz);
                verenigingenPerInszDocument.Verenigingen.Add(vereniging);
                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<List<VerenigingenPerInszDocument>> Apply(NaamWerdGewijzigd naamWerdGewijzigd, IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var documents = await ops.GetVerenigingenPerInszDocuments(naamWerdGewijzigd.VCode);

            foreach (var verenigingenPerInszDocument in documents)
            {
                verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == naamWerdGewijzigd.VCode).Naam = naamWerdGewijzigd.Naam;
                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<VerenigingenPerInszDocument> Apply(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
        {
            var vCode = vertegenwoordigerWerdToegevoegd.StreamKey!;
            var vereniging = await ops.GetVerenigingDocument(vCode);
            var document = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordigerWerdToegevoegd.Data.Insz);

            document.Verenigingen.Add(
                new Vereniging
                {
                    VCode = vereniging.VCode,
                    Naam = vereniging.Naam,
                });
            return document;
        }

        public static async Task<VerenigingenPerInszDocument> Apply(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
        {
            var vCode = vertegenwoordigerWerdVerwijderd.StreamKey!;
            var document = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordigerWerdVerwijderd.Data.Insz);

            document.Verenigingen = document.Verenigingen.Where(v => v.VCode != vCode).ToList();
            return document;
        }
    }

    private static class VerenigingDocumentProjector
    {
        public static VerenigingDocument Apply(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd)
            => new()
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
            };

        public static VerenigingDocument Apply(AfdelingWerdGeregistreerd werdGeregistreerd)
            => new()
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
            };

        public static async Task<VerenigingDocument> Apply(NaamWerdGewijzigd naamWerdGewijzigd, IDocumentOperations ops)
        {
            var verenigingDocument = await ops.GetVerenigingDocument(naamWerdGewijzigd.VCode);
            verenigingDocument.Naam = naamWerdGewijzigd.Naam;
            return verenigingDocument;
        }
    }
}
