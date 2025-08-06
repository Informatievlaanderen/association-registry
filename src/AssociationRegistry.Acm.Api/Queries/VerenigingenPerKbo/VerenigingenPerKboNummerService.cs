namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;

using AssociationRegistry.Magda;
using Marten;

public class VerenigingenPerKboNummerService : IVerenigingenPerKboNummerService
{
    private readonly IRechtsvormCodeService _rechtsvormCodeService;
    private readonly IDocumentStore _store;

    public VerenigingenPerKboNummerService(IRechtsvormCodeService rechtsvormCodeService, IDocumentStore store)
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

        await using var session = _store.LightweightSession();

        var events = await session.Events.FetchStreamAsync(kboNummerMetRechtsvorm.KboNummer);

        if (events.Any())
        {
            var data = events.First().Data as dynamic;
            var vCodeProperty = data.GetType().GetProperty("VCode");
            var vCode = vCodeProperty.GetValue(data);

            return VerenigingenPerKbo.Bekend(kboNummerMetRechtsvorm.KboNummer, vCode);
        }

        return VerenigingenPerKbo.NogNietBekend(kboNummerMetRechtsvorm.KboNummer);
    }
}
