namespace AssociationRegistry.CommandHandling.Persoonsgegevens;

using Admin.MartenDb.VertegenwoordigerPersoonsgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Persoonsgegevens;

public class VertegenwoordigerPersoonsgegevensService : IVertegenwoordigerPersoonsgegevensService
{
    private readonly IVertegenwoordigerPersoonsgegevensQuery _query;

    public VertegenwoordigerPersoonsgegevensService(IVertegenwoordigerPersoonsgegevensQuery query)
    {
        _query = query;
    }

    public async Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId)
    {
        var doc =  await _query.ExecuteAsync(new VertegenwoordigerPersoonsgegevensFilter(refId), CancellationToken.None);

        return new VertegenwoordigerPersoonsgegevens(doc.RefId,
                                                     VCode.Hydrate(doc.VCode),
                                                     doc.VertegenwoordigerId,
                                                     Insz.Hydrate(doc.Insz),
                                                     doc.Roepnaam,
                                                     doc.Rol,
                                                     Voornaam.Hydrate(doc.Voornaam),
                                                     Achternaam.Hydrate(doc.Achternaam),
                                                     doc.Email,
                                                     doc.Telefoon,
                                                     doc.Mobiel,
                                                     doc.SocialMedia);
    }
}
