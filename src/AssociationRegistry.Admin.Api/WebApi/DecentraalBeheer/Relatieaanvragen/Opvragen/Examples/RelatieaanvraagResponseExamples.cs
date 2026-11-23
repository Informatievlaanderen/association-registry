namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.Opvragen.Examples;

using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using DomainRelatieType = DecentraalBeheer.Relatieaanvragen.RelatieType;
using RelatieaanvraagStatus = DecentraalBeheer.Relatieaanvragen.RelatieaanvraagStatus;

public class RelatieaanvraagResponseExamples : IExamplesProvider<RelatieaanvraagResponse>
{
    public RelatieaanvraagResponse GetExamples() => Voorbeeld;

    internal static RelatieaanvraagResponse Voorbeeld =>
        new()
        {
            AanvraagId = "RA0000001",
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            AangevraagdDoor = "V0001001",
            RelatieType = DomainRelatieType.Lidmaatschap.Value,
            Status = RelatieaanvraagStatus.Aangevraagd.Value,
            AanvraagDatum = "2026-11-19",
            VervalDatum = "2027-02-17",
            Beschrijving = "Aanvraag lidmaatschap bij de koepel",
        };
}

public class RelatieaanvragenResponseExamples : IExamplesProvider<RelatieaanvragenResponse>
{
    public RelatieaanvragenResponse GetExamples() =>
        new() { Relatieaanvragen = [RelatieaanvraagResponseExamples.Voorbeeld] };
}
