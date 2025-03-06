namespace AssociationRegistry.Test.Projections;

using Admin.ProjectionHost.Projections.PowerBiExport;
using Admin.Schema.PowerBiExport;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using FluentAssertions.Extensions;
using Marten;
using Npgsql;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereniging;
using Vereniging = Public.Api.Verenigingen.DetailAll.ResponseModels.Vereniging;

public class TestEventsManually
{
    [Fact(Skip = "to decide on later")]
    public async Task TestManually()
    {
        var store = await TestDocumentStoreFactory.CreateAsync("manually");
        await using var session = store.LightweightSession();

        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = fixture.Create<VCode>();

        var daemon = await store.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        // await daemon.StopAllAsync();

        session.Events.StartStream<VerenigingState>(
            vCode,
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode },
            fixture.Create<LidmaatschapWerdToegevoegd>(),
            fixture.Create<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>()
                with
                {
                    VCode = vCode
                });

        await session.SaveChangesAsync();

        await using var conn = store.Storage.Database.CreateConnection();
        await conn.OpenAsync();

        using (var cmd = new NpgsqlCommand(@"
                UPDATE mt_events AS e
                SET version = 27
                where version = 2;", conn))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} event versions updated.");
            rowsAffected.Should().Be(1);
        }

        var state = await session.Events.QueryAllRawEvents().Where(x => x.StreamKey == vCode).ToListAsync();
        state.Single(x => x.Sequence == 2).Version.Should().Be(27);

        // await daemon.StartAllAsync();
        //
        // await daemon.WaitForNonStaleData(20.Seconds());
        // await daemon.RebuildProjectionAsync<PowerBiExportProjection>(20.Seconds(), CancellationToken.None);
        await daemon.WaitForNonStaleData(100.Seconds());

        var powerBiExportDocument = await session.Query<PowerBiExportDocument>()
                                                 .Where(x => x.VCode == vCode.Value)
                                                 .SingleAsync();

        powerBiExportDocument.Verenigingstype.Code.Should().Be(Verenigingstype.VZER.Code);
    }
}
