namespace AssociationRegistry.Services;

using Magda;

public interface IVerenigingenPerKboNummerService
{
    Task<VerenigingenPerKbo[]> GetKboNummerInfo(KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm, CancellationToken cancellationToken);
}

public class VerenigingenPerKboNummerService : IVerenigingenPerKboNummerService
{
    private readonly IRechtsvormCodeService _rechtsvormCodeService;

    public VerenigingenPerKboNummerService(IRechtsvormCodeService rechtsvormCodeService)
    {
        _rechtsvormCodeService = rechtsvormCodeService;
    }

    public static class VCodeUitzonderingen
    {
        public const string NietVanToepassing = "NVT";
        public const string NogNietBekend = "NNB";
    }

    public async Task<VerenigingenPerKbo[]> GetKboNummerInfo(
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


public record VerenigingenPerKbo(string KboNummer, string VCode, bool IsHoofdvertegenwoordiger)
{
    public static VerenigingenPerKbo NietVanToepassing(string kboNummer)
        => new (kboNummer, VerenigingenPerKboNummerService.VCodeUitzonderingen.NietVanToepassing, false);

    public static VerenigingenPerKbo NogNietBekend(string kboNummer)
        => new (kboNummer, VerenigingenPerKboNummerService.VCodeUitzonderingen.NogNietBekend, false);

};
public record KboNummerMetRechtsvorm(string KboNummer, string Rechtsvorm);
