namespace AssociationRegistry.Acm.Api.Projections;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using Events;
using JasperFx.Events;
using Marten;
using Schema.VerenigingenPerInsz;
using Vereniging;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Verenigingstype = DecentraalBeheer.Vereniging.Verenigingstype;

public static class VerenigingDocumentProjector
{
    public static VerenigingDocument Apply(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd)
        => new()
        {
            VCode = werdGeregistreerd.VCode,
            Naam = werdGeregistreerd.Naam,
            Status = VerenigingStatus.Actief,
            VerenigingsType = new(Verenigingstype.FeitelijkeVereniging.Code, Verenigingstype.FeitelijkeVereniging.Naam),
            Verenigingssubtype = null,
            KboNummer = string.Empty,
        };

    public static VerenigingDocument Apply(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
        => new()
        {
            VCode = werdGeregistreerd.VCode,
            Naam = werdGeregistreerd.Naam,
            Status = VerenigingStatus.Actief,
            VerenigingsType = new(Verenigingstype.VZER.Code, Verenigingstype.VZER.Naam),
            Verenigingssubtype = VerenigingssubtypeCode.Default.Map<Verenigingssubtype>(),
            KboNummer = string.Empty,
        };

    public static VerenigingDocument Apply(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
    {
        Verenigingstype verenigingstype = Verenigingstype.Parse(werdGeregistreerd.Rechtsvorm);

        return new()
        {
            VCode = werdGeregistreerd.VCode,
            Naam = werdGeregistreerd.Naam,
            Status = VerenigingStatus.Actief,
            VerenigingsType = new(verenigingstype.Code, verenigingstype.Naam),
            Verenigingssubtype = null,
            KboNummer = werdGeregistreerd.KboNummer,
        };
    }

    public static async Task<VerenigingDocument> Apply(
        IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo,
        IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(rechtsvormWerdGewijzigdInKbo.StreamKey);

        Verenigingstype verenigingstype = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm);

        verenigingDocument.VerenigingsType =
            new(verenigingstype.Code, verenigingstype.Naam);

        return verenigingDocument;
    }

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

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingAanvaarddeDubbeleVereniging> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.CorresponderendeVCodes = verenigingDocument.CorresponderendeVCodes.Append(@event.Data.VCodeDubbeleVereniging).ToArray();

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.CorresponderendeVCodes = verenigingDocument.CorresponderendeVCodes.Where(x => !x.Equals(@event.Data.VCodeDubbeleVereniging, StringComparison.InvariantCultureIgnoreCase)).ToArray();

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.VerenigingsType = new(Verenigingstype.VZER.Code, Verenigingstype.VZER.Naam);

        verenigingDocument.Verenigingssubtype = VerenigingssubtypeCode.Default.Map<Verenigingssubtype>();

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.Verenigingssubtype = VerenigingssubtypeCode.FeitelijkeVereniging.Map<Verenigingssubtype>();

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.Verenigingssubtype = VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>();

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.Verenigingssubtype = VerenigingssubtypeCode.Subvereniging.Map<Verenigingssubtype>();

        return verenigingDocument;
    }
}
