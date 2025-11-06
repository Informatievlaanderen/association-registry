namespace AssociationRegistry.Test.Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;

using Admin.Schema.Persoonsgegevens;
using DecentraalBeheer.Vereniging;
using Persoonsgegevens;

public class VertegenwoordigerPersoonsgegevensRepositoryMock
    : IVertegenwoordigerPersoonsgegevensRepository
{
    private readonly List<VertegenwoordigerPersoonsgegevensDocument> _documents
        = new();

    public List<Guid> SavedRefIds => _documents.Select(x => x.RefId).ToList();

    public Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens)
    {
        _documents.Add(new VertegenwoordigerPersoonsgegevensDocument
        {
            RefId = vertegenwoordigerPersoonsgegevens.RefId,
            VCode = vertegenwoordigerPersoonsgegevens.VCode,
            VertegenwoordigerId = vertegenwoordigerPersoonsgegevens.VertegenwoordigerId,
            Insz = vertegenwoordigerPersoonsgegevens.Insz,
            IsPrimair = vertegenwoordigerPersoonsgegevens.IsPrimair,
            Roepnaam = vertegenwoordigerPersoonsgegevens.Roepnaam,
            Rol = vertegenwoordigerPersoonsgegevens.Rol,
            Voornaam = vertegenwoordigerPersoonsgegevens.Voornaam,
            Achternaam = vertegenwoordigerPersoonsgegevens.Achternaam,
            Email = vertegenwoordigerPersoonsgegevens.Email,
            Telefoon = vertegenwoordigerPersoonsgegevens.Telefoon,
            Mobiel = vertegenwoordigerPersoonsgegevens.Mobiel,
            SocialMedia = vertegenwoordigerPersoonsgegevens.SocialMedia,
        });

        SavedRefIds.Add(vertegenwoordigerPersoonsgegevens.RefId);
        return Task.CompletedTask;
    }

    public async Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId)
    {
        var doc = _documents.FirstOrDefault(x => x.RefId == refId);

        return new VertegenwoordigerPersoonsgegevens(doc.RefId,
                                                     VCode.Hydrate(doc.VCode),
                                                     doc.VertegenwoordigerId,
                                                     Insz.Hydrate(doc.Insz),
                                                     doc.IsPrimair,
                                                     doc.Roepnaam,
                                                     doc.Rol,
                                                     Voornaam.Hydrate(doc.Voornaam),
                                                     Achternaam.Hydrate(doc.Achternaam),
                                                     doc.Email,
                                                     doc.Telefoon,
                                                     doc.Mobiel,
                                                     doc.SocialMedia);
    }

    public IReadOnlyList<VertegenwoordigerPersoonsgegevensDocument> GetAll()
        => _documents.ToList();

    public VertegenwoordigerPersoonsgegevensDocument? FindByRefId(Guid refId)
        => _documents.FirstOrDefault(x => x.RefId == refId);
}

