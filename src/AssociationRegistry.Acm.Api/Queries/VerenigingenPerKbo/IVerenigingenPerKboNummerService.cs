namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;

public interface IVerenigingenPerKboNummerService
{
    Task<VerenigingenPerKbo[]> GetVerenigingenPerKbo(KboNummerMetRechtsvorm[] kboNummersMetRechtsvorm, CancellationToken cancellationToken);
}
