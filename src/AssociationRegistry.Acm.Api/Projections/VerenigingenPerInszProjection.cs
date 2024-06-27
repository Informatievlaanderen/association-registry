namespace AssociationRegistry.Acm.Api.Projections;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Constants;
using Schema.VerenigingenPerInsz;
using System.Collections.Generic;
using System.Linq;
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

    public void Project(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<object>();

        docs.Add(VerenigingDocumentProjector.Apply(werdGeregistreerd));

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

    public async Task Project(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        IDocumentOperations ops)
        => ops.Store(await VerenigingenPerInszProjector.Apply(vertegenwoordigerWerdOvergenomenUitKbo, ops));

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

    private static class VerenigingenPerInszProjector
    {
        public static async Task<List<VerenigingenPerInszDocument>> Apply(
            FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd,
            IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();

            var vereniging = new Vereniging
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
                Status = VerenigingStatus.Actief,
                KboNummer = string.Empty,
                IsHoofdvertegenwoordigerVan = true,
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
                verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == naamWerdGewijzigd.VCode).Naam =
                    naamWerdGewijzigd.Naam;

                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<List<VerenigingenPerInszDocument>> Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var documents = await ops.GetVerenigingenPerInszDocuments(naamWerdGewijzigdInKbo.StreamKey!);

            foreach (var verenigingenPerInszDocument in documents)
            {
                verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == naamWerdGewijzigdInKbo.StreamKey!).Naam =
                    naamWerdGewijzigdInKbo.Data.Naam;

                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<VerenigingenPerInszDocument> Apply(
            IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
            IDocumentOperations ops)
        {
            var vCode = vertegenwoordigerWerdToegevoegd.StreamKey!;
            var vereniging = await ops.GetVerenigingDocument(vCode);
            var document = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordigerWerdToegevoegd.Data.Insz);

            document.Verenigingen.Add(
                new Vereniging
                {
                    VCode = vereniging.VCode,
                    Naam = vereniging.Naam,
                    Status = vereniging.Status,
                    KboNummer = vereniging.KboNummer,
                    IsHoofdvertegenwoordigerVan = true,
                });

            return document;
        }

        public static async Task<VerenigingenPerInszDocument> Apply(
            IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd,
            IDocumentOperations ops)
        {
            var vCode = vertegenwoordigerWerdVerwijderd.StreamKey!;
            var document = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordigerWerdVerwijderd.Data.Insz);

            document.Verenigingen = document.Verenigingen.Where(v => v.VCode != vCode).ToList();

            return document;
        }

        public static async Task<VerenigingenPerInszDocument> Apply(
            IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
            IDocumentOperations ops)
        {
            var vCode = vertegenwoordigerWerdOvergenomenUitKbo.StreamKey!;
            var vereniging = await ops.GetVerenigingDocument(vCode);
            var document = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordigerWerdOvergenomenUitKbo.Data.Insz);

            document.Verenigingen.Add(
                new Vereniging
                {
                    VCode = vereniging.VCode,
                    Naam = vereniging.Naam,
                    Status = vereniging.Status,
                    KboNummer = vereniging.KboNummer,
                    IsHoofdvertegenwoordigerVan = true,
                });

            return document;
        }

        public static async Task<List<VerenigingenPerInszDocument>> Apply(
            IEvent<VerenigingWerdGestopt> verenigingWerdGestopt,
            IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var documents = await ops.GetVerenigingenPerInszDocuments(verenigingWerdGestopt.StreamKey!);

            foreach (var verenigingenPerInszDocument in documents)
            {
                verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == verenigingWerdGestopt.StreamKey!).Status =
                    VerenigingStatus.Gestopt;

                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }

        public static async Task<List<VerenigingenPerInszDocument>> Apply(
            IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd,
            IDocumentOperations ops)
        {
            var docs = new List<VerenigingenPerInszDocument>();
            var documents = await ops.GetVerenigingenPerInszDocuments(verenigingWerdVerwijderd.StreamKey!);

            foreach (var verenigingenPerInszDocument in documents)
            {
                verenigingenPerInszDocument.Verenigingen = verenigingenPerInszDocument.Verenigingen
                                                                                      .Where(v => v.VCode !=
                                                                                               verenigingWerdVerwijderd.StreamKey)
                                                                                      .ToList();

                docs.Add(verenigingenPerInszDocument);
            }

            return docs;
        }
    }

    private static class VerenigingDocumentProjector
    {
        public static VerenigingDocument Apply(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd)
            => new()
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
                Status = VerenigingStatus.Actief,
                KboNummer = string.Empty,
            };

        public static VerenigingDocument Apply(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
            => new()
            {
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
                Status = VerenigingStatus.Actief,
                KboNummer = werdGeregistreerd.KboNummer,
            };

        public static async Task<VerenigingDocument> Apply(NaamWerdGewijzigd naamWerdGewijzigd, IDocumentOperations ops)
        {
            var verenigingDocument = await ops.GetVerenigingDocument(naamWerdGewijzigd.VCode);
            verenigingDocument.Naam = naamWerdGewijzigd.Naam;

            return verenigingDocument;
        }

        public static async Task<VerenigingDocument> Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, IDocumentOperations ops)
        {
            var verenigingDocument = await ops.GetVerenigingDocument(naamWerdGewijzigdInKbo.StreamKey!);
            verenigingDocument.Naam = naamWerdGewijzigdInKbo.Data.Naam;

            return verenigingDocument;
        }

        public static async Task<VerenigingDocument> Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, IDocumentOperations ops)
        {
            var verenigingDocument = await ops.GetVerenigingDocument(verenigingWerdGestopt.StreamKey!);
            verenigingDocument.Status = VerenigingStatus.Gestopt;

            return verenigingDocument;
        }

        public static async Task<VerenigingDocument> Apply(
            IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd,
            IDocumentOperations ops)
        {
            ops.Delete<VerenigingDocument>(verenigingWerdVerwijderd.StreamKey!);

            return await ops.GetVerenigingDocument(verenigingWerdVerwijderd.StreamKey!);
        }
    }
}
