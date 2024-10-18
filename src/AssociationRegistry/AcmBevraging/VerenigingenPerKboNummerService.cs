namespace AssociationRegistry.AcmBevraging;

using Magda;

public class VerenigingenPerKboNummerService : IVerenigingenPerKboNummerService
{
    private readonly IRechtsvormCodeService _rechtsvormCodeService;

    public VerenigingenPerKboNummerService(IRechtsvormCodeService rechtsvormCodeService)
    {
        _rechtsvormCodeService = rechtsvormCodeService;
    }

    public async Task<VerenigingenPerKbo[]> GetVerenigingenPerKbo(
        KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm,
        CancellationToken cancellationToken)
    {
        var result = new List<VerenigingenPerKbo>();

        foreach (var kboNummerMetRechtsvorm in kboNummersMetRechtsvorm)
        {
            result.Add(Process(kboNummerMetRechtsvorm));
        }

        return result.ToArray();
    }

    private VerenigingenPerKbo Process(KboNummerMetRechtsvorm kboNummerMetRechtsvorm)
    {
        if (!_rechtsvormCodeService.IsValidRechtsvormCode(kboNummerMetRechtsvorm.Rechtsvorm))
            return VerenigingenPerKbo.NietVanToepassing(kboNummerMetRechtsvorm.KboNummer);

        return VerenigingenPerKbo.NogNietBekend(kboNummerMetRechtsvorm.KboNummer);
    }
}
