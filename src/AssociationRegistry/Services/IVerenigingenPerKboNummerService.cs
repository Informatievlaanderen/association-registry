namespace AssociationRegistry.Services;

public interface IVerenigingenPerKboNummerService
{
    Task<VerenigingenPerKbo[]> GetKboNummerInfo(KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm, CancellationToken cancellationToken);
}

public class VerenigingenPerKboNummerService : IVerenigingenPerKboNummerService
{
    public async Task<VerenigingenPerKbo[]> GetKboNummerInfo(
        KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm,
        CancellationToken cancellationToken)
    {
        var result = new List<VerenigingenPerKbo>();

        foreach (var kboNummerMetRechtsvorm in kboNummersMetRechtsvorm)
        {
            if (HeeftGeldigeRechtsvorm(kboNummerMetRechtsvorm))
            {
            }
            else
            {
                result.Add(new(kboNummerMetRechtsvorm.KboNummer, "NVT", false));
            }
        }

        return result.ToArray();
    }

    private bool HeeftGeldigeRechtsvorm(KboNummerMetRechtsvorm kboNummerMetRechtsvorm)
    {
        return false;
    }
}

public record VerenigingenPerKbo(string KboNummer, string VCode, bool IsHoofdvertegenwoordiger);
public record KboNummerMetRechtsvorm(string KboNummer, string Rechtsvorm);
