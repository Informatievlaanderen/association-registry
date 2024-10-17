namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerKboNummer;

public interface IVerenigingenPerKboNummerService
{
    Task<KboNummerInfo[]> GetKboNummerInfo();
}

public record KboNummerInfo(string KboNummer, string VCode, bool IsHoofdvertegenwoordiger);
