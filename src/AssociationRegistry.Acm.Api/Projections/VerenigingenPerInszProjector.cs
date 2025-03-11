namespace AssociationRegistry.Acm.Api.Projections;

using Events;
using Marten;
using Marten.Events;
using Schema.Constants;
using Schema.VerenigingenPerInsz;

public static class VerenigingenPerInszProjector
{
    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();

        foreach (var vertegenwoordiger in werdGeregistreerd.Vertegenwoordigers)
        {
            var vereniging = new Vereniging
            {
                VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                VCode = werdGeregistreerd.VCode,
                Naam = werdGeregistreerd.Naam,
                Status = VerenigingStatus.Actief,
                KboNummer = string.Empty,
                Verenigingstype = MapVerenigingstype(werdGeregistreerd),
                Verenigingssubtype = MapVerenigingssubtype(werdGeregistreerd),
                IsHoofdvertegenwoordigerVan = true,
            };

            var verenigingenPerInszDocument = await ops.GetVerenigingenPerInszDocumentOrNew(vertegenwoordiger.Insz);
            verenigingenPerInszDocument.Verenigingen.Add(vereniging);
            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    private static Verenigingstype MapVerenigingstype(IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
    {
        return werdGeregistreerd switch
        {
            FeitelijkeVerenigingWerdGeregistreerd =>
                new(
                    AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                    AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam),

            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =>
                new(
                    AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                    AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam),

            _ => throw new ArgumentOutOfRangeException(nameof(werdGeregistreerd))
        };
    }

    private static Verenigingssubtype? MapVerenigingssubtype(IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
    {
        return werdGeregistreerd switch
        {
            FeitelijkeVerenigingWerdGeregistreerd => null,

            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =>
                new Verenigingssubtype
                {
                    Code = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Code,
                    Naam = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Naam,

                },

            _ => throw new ArgumentOutOfRangeException(nameof(werdGeregistreerd)),
        };
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

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo,
        IDocumentOperations ops)
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
                VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId,
                VCode = vereniging.VCode,
                Naam = vereniging.Naam,
                Status = vereniging.Status,
                KboNummer = vereniging.KboNummer,
                Verenigingstype = vereniging.VerenigingsType,
                Verenigingssubtype = vereniging.Verenigingssubtype,
                IsHoofdvertegenwoordigerVan = true,
                CorresponderendeVCodes = vereniging.CorresponderendeVCodes,
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
                VertegenwoordigerId = vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId,
                Naam = vereniging.Naam,
                Status = vereniging.Status,
                KboNummer = vereniging.KboNummer,
                Verenigingstype = vereniging.VerenigingsType,
                Verenigingssubtype = vereniging.Verenigingssubtype,
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

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(rechtsvormWerdGewijzigdInKbo.StreamKey!);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging = verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == rechtsvormWerdGewijzigdInKbo.StreamKey!);

            vereniging.Verenigingstype = MapVereniging(AssociationRegistry.Vereniging.Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm));

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<VerenigingAanvaarddeDubbeleVereniging> verenigingAanvaarddeDubbeleVereniging,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(verenigingAanvaarddeDubbeleVereniging.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == verenigingAanvaarddeDubbeleVereniging.StreamKey!);

            vereniging.CorresponderendeVCodes =
                vereniging.CorresponderendeVCodes
                          .Append(verenigingAanvaarddeDubbeleVereniging.Data.VCodeDubbeleVereniging)
                          .ToArray();

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> verenigingWerdGemarkeerdAlsDubbelVan,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(verenigingWerdGemarkeerdAlsDubbelVan.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == verenigingWerdGemarkeerdAlsDubbelVan.StreamKey!);

            vereniging.IsDubbel = true;

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> weigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(weigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == weigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.StreamKey!);

            vereniging.IsDubbel = false;

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> verenigingAanvaarddeCorrectieDubbeleVereniging,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(verenigingAanvaarddeCorrectieDubbeleVereniging.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == verenigingAanvaarddeCorrectieDubbeleVereniging.StreamKey!);

            vereniging.CorresponderendeVCodes =
                vereniging.CorresponderendeVCodes
                          .Where(w => w != verenigingAanvaarddeCorrectieDubbeleVereniging.Data.VCodeDubbeleVereniging)
                          .ToArray();

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> markeringDubbeleVerengingWerdGecorrigeerd,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(markeringDubbeleVerengingWerdGecorrigeerd.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == markeringDubbeleVerengingWerdGecorrigeerd.StreamKey!);

            vereniging.IsDubbel = false;

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(@event.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == @event.StreamKey!);

            vereniging.Verenigingstype = new(
                AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam);

            vereniging.Verenigingssubtype = new Verenigingssubtype
            {
                Code = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Naam,
            };

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    public static async Task<List<VerenigingenPerInszDocument>> Apply(
        IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event,
        IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        var documents = await ops.GetVerenigingenPerInszDocuments(@event.Data.VCode);

        foreach (var verenigingenPerInszDocument in documents)
        {
            var vereniging =
                verenigingenPerInszDocument.Verenigingen.Single(
                    vereniging => vereniging.VCode == @event.StreamKey!);

            vereniging.Verenigingssubtype = new Verenigingssubtype
            {
                Code = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam,
            };

            docs.Add(verenigingenPerInszDocument);
        }

        return docs;
    }

    private static Verenigingstype MapVereniging(AssociationRegistry.Vereniging.Verenigingstype verenigingstype)
        => new(verenigingstype.Code, verenigingstype.Naam);

}
