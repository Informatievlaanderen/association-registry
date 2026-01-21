namespace AssociationRegistry.Persoonsgegevens;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bankrekeningen;

public interface IBankrekeningnummerPersoonsgegevensRepository
{
    Task Save(BankrekeningnummerPersoonsgegevens bankrekeningnummerPersoonsgegevens);
    Task<BankrekeningnummerPersoonsgegevens> Get(Guid refId, CancellationToken cancellationToken);
    Task<BankrekeningnummerPersoonsgegevens[]> Get(Guid[] refId, CancellationToken cancellationToken);
    Task<BankrekeningnummerPersoonsgegevens[]> Get(IbanNummer ibanNummer, CancellationToken cancellationToken);
}
