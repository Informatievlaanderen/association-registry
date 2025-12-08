namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> UpcastAsync(
        KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens vertegenwoordigerWerdVerwijderd,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var refId = vertegenwoordigerWerdVerwijderd.RefId;
        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == refId)
                                                             .SingleOrDefaultAsync(ct);

        return new KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
            VertegenwoordigerId: vertegenwoordigerWerdVerwijderd.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
