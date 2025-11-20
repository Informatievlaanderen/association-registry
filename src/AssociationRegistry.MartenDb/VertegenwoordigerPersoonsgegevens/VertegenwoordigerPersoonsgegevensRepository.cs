namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerPersoonsgegevensRepository : IVertegenwoordigerPersoonsgegevensRepository
{
    private readonly IDocumentSession _session;
    private readonly IVertegenwoordigerPersoonsgegevensQuery _vertegenwoordigerPersoonsgegevensQuery;

    public VertegenwoordigerPersoonsgegevensRepository(
        IDocumentSession session,
        IVertegenwoordigerPersoonsgegevensQuery vertegenwoordigerPersoonsgegevensQuery)
    {
        _session = session;
        _vertegenwoordigerPersoonsgegevensQuery = vertegenwoordigerPersoonsgegevensQuery;
    }

    public async Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens)
    {
        _session.Insert(new VertegenwoordigerPersoonsgegevensDocument
        {
            RefId = vertegenwoordigerPersoonsgegevens.RefId,
            VCode = vertegenwoordigerPersoonsgegevens.VCode,
            VertegenwoordigerId = vertegenwoordigerPersoonsgegevens.VertegenwoordigerId,
            Insz = vertegenwoordigerPersoonsgegevens.Insz,
            Roepnaam = vertegenwoordigerPersoonsgegevens.Roepnaam,
            Rol = vertegenwoordigerPersoonsgegevens.Rol,
            Voornaam = vertegenwoordigerPersoonsgegevens.Voornaam,
            Achternaam = vertegenwoordigerPersoonsgegevens.Achternaam,
            Email = vertegenwoordigerPersoonsgegevens.Email,
            Telefoon = vertegenwoordigerPersoonsgegevens.Telefoon,
            Mobiel = vertegenwoordigerPersoonsgegevens.Mobiel,
            SocialMedia = vertegenwoordigerPersoonsgegevens.SocialMedia,
        });
    }

    public async Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId, CancellationToken cancellationToken)
    {
        var vertegenwoordigerPersoonsgegevens =
            await _vertegenwoordigerPersoonsgegevensQuery.ExecuteAsync(new VertegenwoordigerPersoonsgegevensByRefIdFilter(refId),
                                                                       cancellationToken);

        return new VertegenwoordigerPersoonsgegevens(vertegenwoordigerPersoonsgegevens.RefId,
                                                     VCode.Hydrate(vertegenwoordigerPersoonsgegevens.VCode),
                                                     vertegenwoordigerPersoonsgegevens.VertegenwoordigerId,
                                                     Insz.Hydrate(vertegenwoordigerPersoonsgegevens.Insz),
                                                     vertegenwoordigerPersoonsgegevens.Roepnaam,
                                                     vertegenwoordigerPersoonsgegevens.Rol,
                                                     vertegenwoordigerPersoonsgegevens.Voornaam,
                                                     vertegenwoordigerPersoonsgegevens.Achternaam,
                                                     vertegenwoordigerPersoonsgegevens.Email,
                                                     vertegenwoordigerPersoonsgegevens.Telefoon,
                                                     vertegenwoordigerPersoonsgegevens.Mobiel,
                                                     vertegenwoordigerPersoonsgegevens.SocialMedia
        );
    }

    public async Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refIds, CancellationToken cancellationToken)
    {
        var vertegenwoordigerPersoonsgegevens =
            await _vertegenwoordigerPersoonsgegevensQuery.ExecuteAsync(new VertegenwoordigerPersoonsgegevensByRefIdsFilter(refIds),
                                                                       cancellationToken);

        return vertegenwoordigerPersoonsgegevens.Select(v => new VertegenwoordigerPersoonsgegevens(
                                                            v.RefId,
                                                            VCode.Hydrate(v.VCode),
                                                            v.VertegenwoordigerId,
                                                            Insz.Hydrate(v.Insz),
                                                            v.Roepnaam,
                                                            v.Rol,
                                                            v.Voornaam,
                                                            v.Achternaam,
                                                            v.Email,
                                                            v.Telefoon,
                                                            v.Mobiel,
                                                            v.SocialMedia
                                                        )).ToArray();
    }
}
