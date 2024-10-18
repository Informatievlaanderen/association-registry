namespace AssociationRegistry.AcmBevraging;

public interface IVerenigingenPerKboNummerService
{
    Task<VerenigingenPerKbo[]> GetVerenigingenPerKbo(KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm, CancellationToken cancellationToken);
}
