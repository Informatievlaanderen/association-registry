namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Persoonsgegevens;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using Marten;

public class BankrekeningnummerPersoonsgegevensRepository : IBankrekeningnummerPersoonsgegevensRepository
{
    private readonly IDocumentSession _session;
    private readonly IBankrekeningnummerPersoonsgegevensQuery _vertegenwoordigerPersoonsgegevensQuery;

    public BankrekeningnummerPersoonsgegevensRepository(
        IDocumentSession session,
        IBankrekeningnummerPersoonsgegevensQuery vertegenwoordigerPersoonsgegevensQuery)
    {
        _session = session;
        _vertegenwoordigerPersoonsgegevensQuery = vertegenwoordigerPersoonsgegevensQuery;
    }

    public async Task Save(BankrekeningnummerPersoonsgegevens vertegenwoordigerPersoonsgegevens)
    {
        _session.Insert(new BankrekeningnummerPersoonsgegevensDocument
        {
            RefId = vertegenwoordigerPersoonsgegevens.RefId,
            VCode = vertegenwoordigerPersoonsgegevens.VCode,
            BankrekeningnummerId = vertegenwoordigerPersoonsgegevens.BankrekeningnummerId,
            Iban = vertegenwoordigerPersoonsgegevens.Iban,
            Titularis = vertegenwoordigerPersoonsgegevens.Titularis
        });
    }

    public async Task<BankrekeningnummerPersoonsgegevens> Get(Guid refId, CancellationToken cancellationToken)
    {
        var persoonsgegevens =
            await _vertegenwoordigerPersoonsgegevensQuery.ExecuteAsync(new BankrekeningnummerPersoonsgegevensByRefIdFilter(refId),
                                                                       cancellationToken);

        return new BankrekeningnummerPersoonsgegevens(persoonsgegevens.RefId,
                                                     VCode.Hydrate(persoonsgegevens.VCode),
                                                     persoonsgegevens.BankrekeningnummerId,
                                                     IbanNummer.Hydrate(persoonsgegevens.Iban).Value,
                                                     persoonsgegevens.Titularis
        );
    }

    public async Task<BankrekeningnummerPersoonsgegevens[]> Get(Guid[] refIds, CancellationToken cancellationToken)
    {
        var persoonsgegevens =
            await _vertegenwoordigerPersoonsgegevensQuery.ExecuteAsync(new BankrekeningnummerPersoonsgegevensByRefIdsFilter(refIds),
                                                                       cancellationToken);

        return persoonsgegevens.Select(v => new BankrekeningnummerPersoonsgegevens(
                                                            v.RefId,
                                                            VCode.Hydrate(v.VCode),
                                                            v.BankrekeningnummerId,
                                                            IbanNummer.Hydrate(v.Iban).Value,
                                                            v.Titularis
                                                        )).ToArray();
    }

    public async Task<BankrekeningnummerPersoonsgegevens[]> Get(IbanNummer iban, CancellationToken cancellationToken)
    {
        var vertegenwoordigerPersoonsgegevens =
            await _vertegenwoordigerPersoonsgegevensQuery.ExecuteAsync(new BankrekeningnummerPersoonsgegevensByIbanFilter(iban),
                                                                       cancellationToken);

        return vertegenwoordigerPersoonsgegevens.Select(v => new BankrekeningnummerPersoonsgegevens(
                                                            v.RefId,
                                                            VCode.Hydrate(v.VCode),
                                                            v.BankrekeningnummerId,
                                                            IbanNummer.Hydrate(v.Iban).Value,
                                                            v.Titularis
                                                        )).ToArray();
    }
}
