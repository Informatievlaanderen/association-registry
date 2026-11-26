namespace AssociationRegistry.Test.Scheduled.Host.Queries;

using Admin.Schema.Relatieaanvragen;
using AssociationRegistry.Scheduled.Host.Queries;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using DecentraalBeheer.Relatieaanvragen;
using FluentAssertions;
using Marten;

public class TeVervallenRelatieaanvragenQueryTests
{
    private static readonly DateOnly Today = new(2026, 11, 26);

    [Fact]
    public async ValueTask Returns_Only_Aangevraagde_Relatieaanvragen_Whose_VervalDatum_Is_Before_Today()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeVervallenRelatieaanvragenQueryTests));
        var session = store.LightweightSession();

        session.StoreObjects([
            Document(1, RelatieaanvraagStatus.Aangevraagd, vervalDatum: Today.AddDays(-1)),
            Document(2, RelatieaanvraagStatus.Aangevraagd, vervalDatum: Today.AddDays(-30)),
            Document(3, RelatieaanvraagStatus.Aangevraagd, vervalDatum: Today),
            Document(4, RelatieaanvraagStatus.Aangevraagd, vervalDatum: Today.AddDays(1)),
            Document(5, RelatieaanvraagStatus.Bevestigd, vervalDatum: Today.AddDays(-1)),
            Document(6, RelatieaanvraagStatus.Geweigerd, vervalDatum: Today.AddDays(-1)),
            Document(7, RelatieaanvraagStatus.Ingetrokken, vervalDatum: Today.AddDays(-1)),
            Document(8, RelatieaanvraagStatus.Vervallen, vervalDatum: Today.AddDays(-1)),
        ]);

        await session.SaveChangesAsync();

        var sut = new TeVervallenRelatieaanvragenQuery(session, new ClockStub(Today));
        var actual = await sut.ExecuteAsync(CancellationToken.None);

        actual.Select(relatieaanvraag => relatieaanvraag.AanvraagId).Should().BeEquivalentTo("RA0000001", "RA0000002");
    }

    private static RelatieaanvraagDocument Document(long nummer, RelatieaanvraagStatus status, DateOnly vervalDatum) =>
        new()
        {
            AanvraagId = RelatieaanvraagId.Create(nummer).Value,
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            AangevraagdDoor = "V0001001",
            RelatieType = RelatieType.Lidmaatschap.Value,
            Status = status.Value,
            AanvraagDatum = vervalDatum.AddDays(-90),
            VervalDatum = vervalDatum,
        };
}
