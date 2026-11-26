namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.Opvragen.Examples;

using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using DomainRelatieType = DecentraalBeheer.Relatieaanvragen.RelatieType;
using RelatieaanvraagStatus = DecentraalBeheer.Relatieaanvragen.RelatieaanvraagStatus;

public class RelatieaanvraagResponseExamples : IExamplesProvider<RelatieaanvraagResponse>
{
    public RelatieaanvraagResponse GetExamples() => Bevestigd;

    internal static RelatieaanvraagResponse Aangevraagd =>
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
            AfhandelDatum = string.Empty,
        };

    internal static RelatieaanvraagResponse Bevestigd =>
        new()
        {
            AanvraagId = "RA0000002",
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001003",
            AangevraagdDoor = "V0001003",
            RelatieType = DomainRelatieType.Subvereniging.Value,
            Status = RelatieaanvraagStatus.Bevestigd.Value,
            AanvraagDatum = "2026-11-19",
            VervalDatum = "2027-02-17",
            Beschrijving = "Wij zijn de afdeling Bree",
            AfhandelDatum = "2026-11-23",
        };

    internal static RelatieaanvraagResponse Geweigerd =>
        new()
        {
            AanvraagId = "RA0000003",
            KoepelVCode = "V0001004",
            VerenigingVCode = "V0001003",
            AangevraagdDoor = "V0001004",
            RelatieType = DomainRelatieType.Lidmaatschap.Value,
            Status = RelatieaanvraagStatus.Geweigerd.Value,
            AanvraagDatum = "2026-11-20",
            VervalDatum = "2027-02-18",
            Beschrijving = "Aanvraag lidmaatschap bij de koepel",
            AfhandelDatum = "2026-11-24",
        };

    internal static RelatieaanvraagResponse Ingetrokken =>
        new()
        {
            AanvraagId = "RA0000004",
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001005",
            AangevraagdDoor = "V0001005",
            RelatieType = DomainRelatieType.Subvereniging.Value,
            Status = RelatieaanvraagStatus.Ingetrokken.Value,
            AanvraagDatum = "2026-11-21",
            VervalDatum = "2027-02-19",
            Beschrijving = "Wij zijn de afdeling Bree",
            AfhandelDatum = "2026-11-24",
        };

    /// <summary>The afhandelDatum of a vervallen relatieaanvraag is the day after the vervalDatum.</summary>
    internal static RelatieaanvraagResponse Vervallen =>
        new()
        {
            AanvraagId = "RA0000005",
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001006",
            AangevraagdDoor = "V0001001",
            RelatieType = DomainRelatieType.Lidmaatschap.Value,
            Status = RelatieaanvraagStatus.Vervallen.Value,
            AanvraagDatum = "2026-08-20",
            VervalDatum = "2026-11-18",
            Beschrijving = "Aanvraag lidmaatschap bij de koepel",
            AfhandelDatum = "2026-11-19",
        };
}

public class RelatieaanvragenResponseExamples : IExamplesProvider<RelatieaanvragenResponse>
{
    public RelatieaanvragenResponse GetExamples() =>
        new()
        {
            Relatieaanvragen =
            [
                RelatieaanvraagResponseExamples.Aangevraagd,
                RelatieaanvraagResponseExamples.Bevestigd,
                RelatieaanvraagResponseExamples.Geweigerd,
                RelatieaanvraagResponseExamples.Ingetrokken,
                RelatieaanvraagResponseExamples.Vervallen,
            ],
        };
}
