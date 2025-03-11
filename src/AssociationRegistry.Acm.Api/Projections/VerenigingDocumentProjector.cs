namespace AssociationRegistry.Acm.Api.Projections;

using Events;
using Marten;
using Marten.Events;
using Schema.Constants;
using Schema.VerenigingenPerInsz;

public static class VerenigingDocumentProjector
{
    public static VerenigingDocument Apply(FeitelijkeVerenigingWerdGeregistreerd werdGeregistreerd)
        => new()
        {
            VCode = werdGeregistreerd.VCode,
            Naam = werdGeregistreerd.Naam,
            Status = VerenigingStatus.Actief,
            VerenigingsType = new(AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code, AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam),
            Verenigingssubtype = null,
            KboNummer = string.Empty,
        };

    public static VerenigingDocument Apply(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
        => new()
        {
            VCode = werdGeregistreerd.VCode,
            Naam = werdGeregistreerd.Naam,
            Status = VerenigingStatus.Actief,
            VerenigingsType = new(AssociationRegistry.Vereniging.Verenigingstype.VZER.Code, AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam),
            Verenigingssubtype = new Verenigingssubtype()
            {
                Code = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Naam,
            },
            KboNummer = string.Empty,
        };

    public static VerenigingDocument Apply(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd werdGeregistreerd)
    {
        AssociationRegistry.Vereniging.Verenigingstype verenigingstype = AssociationRegistry.Vereniging.Verenigingstype.Parse(werdGeregistreerd.Rechtsvorm);

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

        AssociationRegistry.Vereniging.Verenigingstype verenigingstype = AssociationRegistry.Vereniging.Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm);

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

        verenigingDocument.VerenigingsType = new(AssociationRegistry.Vereniging.Verenigingstype.VZER.Code, AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam);

        verenigingDocument.Verenigingssubtype = new Verenigingssubtype()
        {
            Code = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Code,
            Naam = AssociationRegistry.Vereniging.Verenigingssubtype.NietBepaald.Naam,
        };

        return verenigingDocument;
    }

    public static async Task<VerenigingDocument> Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event, IDocumentOperations ops)
    {
        var verenigingDocument = await ops.GetVerenigingDocument(@event.StreamKey!);

        verenigingDocument.Verenigingssubtype = new Verenigingssubtype()
        {
            Code = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code,
            Naam = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam,
        };

        return verenigingDocument;
    }
}
