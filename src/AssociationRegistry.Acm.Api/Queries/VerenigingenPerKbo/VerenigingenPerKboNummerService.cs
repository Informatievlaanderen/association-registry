namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;

using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using EventStore;
using Marten;
using MartenDb.Store;

public class VerenigingenPerKboNummerService : IVerenigingenPerKboNummerService
{
    private readonly IRechtsvormCodeService _rechtsvormCodeService;
    private readonly IEventStore _store;

    public VerenigingenPerKboNummerService(IRechtsvormCodeService rechtsvormCodeService, IEventStore store)
    {
        _rechtsvormCodeService = rechtsvormCodeService;
        _store = store;
    }

    public async Task<VerenigingenPerKbo[]> GetVerenigingenPerKbo(
        KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm,
        CancellationToken cancellationToken)
    {
        var result = new List<VerenigingenPerKbo>();

        foreach (var kboNummerMetRechtsvorm in kboNummersMetRechtsvorm)
        {
            result.Add(await Process(kboNummerMetRechtsvorm));
        }

        return result.ToArray();
    }

    private async Task<VerenigingenPerKbo> Process(KboNummerMetRechtsvorm kboNummerMetRechtsvorm)
    {
        if (!_rechtsvormCodeService.IsValidRechtsvormCode(kboNummerMetRechtsvorm.Rechtsvorm))
            return VerenigingenPerKbo.NietVanToepassing(kboNummerMetRechtsvorm.KboNummer);

        var vCode = await _store.GetVCodeForKbo(kboNummerMetRechtsvorm.KboNummer);

        if (vCode is null)
            return VerenigingenPerKbo.NogNietBekend(kboNummerMetRechtsvorm.KboNummer);

        return VerenigingenPerKbo.Bekend(kboNummerMetRechtsvorm.KboNummer, vCode!);
    }
}
