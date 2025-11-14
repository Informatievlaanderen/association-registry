namespace AssociationRegistry.Admin.MartenDb.VertegenwoordigerPersoonsgegevens;

using Marten;
using Persoonsgegevens;
using Schema.Persoonsgegevens;

public class VertegenwoordigerPersoonsgegevensRepository: IVertegenwoordigerPersoonsgegevensRepository
{
    private readonly IDocumentSession _session;
    private readonly IVertegenwoordigerPersoonsgegevensService _vertegenwoordigerPersoonsgegevensService;

    public VertegenwoordigerPersoonsgegevensRepository(IDocumentSession session, IVertegenwoordigerPersoonsgegevensService vertegenwoordigerPersoonsgegevensService)
    {
        _session = session;
        _vertegenwoordigerPersoonsgegevensService = vertegenwoordigerPersoonsgegevensService;
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

    public async Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId)
        => await _vertegenwoordigerPersoonsgegevensService.Get(refId);

    public async Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refId)
        => await _vertegenwoordigerPersoonsgegevensService.Get(refId);
}
