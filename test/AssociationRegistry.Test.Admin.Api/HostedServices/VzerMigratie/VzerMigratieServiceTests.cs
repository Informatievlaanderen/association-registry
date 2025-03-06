namespace AssociationRegistry.Test.Admin.Api.HostedServices.VzerMigratie;

using AssociationRegistry.Admin.Api.HostedServices.VzerMigratie;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using EventStore;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NodaTime.Text;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vereniging;
using Xunit;

public class VzerMigratieServiceTests
{
    [Fact]
    public async Task RunMultipleTimes()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var store = await TestDocumentStoreFactory.CreateAsync("vzermigraties");

        await using var sessionForSetup = store.LightweightSession();

        var vcodeNrs = Enumerable.Range(1001,50);

        SetHeaders(new CommandMetadata(AssociationRegistry.EventStore.EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                       null), sessionForSetup);

        foreach (var vcodeNr in vcodeNrs)
        {
            sessionForSetup.Events.Append(VCode.Create(vcodeNr), CreateSemiRandomEvents(fixture, vcodeNr));
        }

        await sessionForSetup.SaveChangesAsync();

        await using var sessionForAssert = store.LightweightSession();

        var tasks = Enumerable.Repeat(new VzerMigratieService(store, NullLogger<VzerMigratieService>.Instance), 3).Select(x =>
        {
            x.StartAsync(CancellationToken.None);
            return x.ExecuteTask;
        }).ToArray();

        await Task.WhenAll(tasks!);

        foreach (var vcodeNr in vcodeNrs)
        {
            var stream = await sessionForAssert.Events.FetchStreamAsync(VCode.Create(vcodeNr));
            var migratieEvents = stream.Where(x => x.EventType == typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid));
            migratieEvents.Count().Should().BeLessThan(2);
            var lastEvent = stream.Last();
            lastEvent.EventType.Should().Be(GetExpectedEventForStream(vcodeNr));
            lastEvent.Headers["Tijdstip"].Should().NotBeNull();
            lastEvent.Headers["Initiator"].Should().Be(EventStore.DigitaalVlaanderenOvoNumber);
            lastEvent.CorrelationId.Should().NotBeNullOrEmpty();
        }

        var m = sessionForAssert.Events.QueryAllRawEvents().Max(x => x.Sequence);
        m.Should().Be(101);
    }

    private static object[] CreateSemiRandomEvents(Fixture fixture, int vcodeNr)
    {
        return vcodeNr switch
        {
            var n when n % 50 == 0 => [fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()with{VCode = VCode.Create(vcodeNr)}, fixture.Create<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>(), fixture.Create<LocatieWerdToegevoegd>()],
            var n when n % 40 == 0 => [fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()with{VCode = VCode.Create(vcodeNr)}],
            var n when n % 20 == 0 => [fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()with{VCode = VCode.Create(vcodeNr)}],
            var n when n % 10 == 0 => [fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()with{VCode = VCode.Create(vcodeNr)}, fixture.Create<ContactgegevenWerdToegevoegd>()],
            var n when n % 5 == 0 => [fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()with{VCode = VCode.Create(vcodeNr)}, fixture.Create<ContactgegevenWerdToegevoegd>()],
            _ => [fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with{VCode = VCode.Create(vcodeNr)}],
        };
    }

    private static Type GetExpectedEventForStream(int vcodeNr)
    {
        return vcodeNr switch
        {
            var n when n % 50 == 0 => typeof(LocatieWerdToegevoegd),
            var n when n % 40 == 0 => typeof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
            var n when n % 20 == 0 => typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
            var n when n % 10 == 0 => typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
            var n when n % 5 == 0 => typeof(ContactgegevenWerdToegevoegd),
            _ => typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
        };
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        session.CorrelationId = metadata.CorrelationId.ToString();
    }
}
