namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerKboNummer;

using Api.VerenigingenPerInsz;

public interface IVerenigingenPerKboNummerService
{
    Task<VerenigingenPerKbo[]> GetKboNummerInfo(VerenigingenPerInszRequest.KboRequest[] kboRequest);
}

public record VerenigingenPerKbo(string KboNummer, string VCode, bool IsHoofdvertegenwoordiger);
