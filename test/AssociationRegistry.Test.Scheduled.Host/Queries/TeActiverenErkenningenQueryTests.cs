namespace AssociationRegistry.Test.Scheduled.Host.Queries;

using System.Globalization;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Scheduled.Host.Erkenningen;
using AssociationRegistry.Test.Common.Framework;
using FluentAssertions;
using Marten;
using NodaTime;
using DomainErkenningStatus = AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.ErkenningStatus;

public class TeActiverenErkenningenQueryTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);

    [Fact]
    public async ValueTask Returns_Only_Erkenningen_In_Aanvraag_With_Startdatum_In_The_Past_Or_Today()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeActiverenErkenningenQueryTests));
        var session = store.LightweightSession();

        await InsertVerenigingen(session);

        var sut = new TeActiverenErkenningenQuery(session, new FakeClock(Instant.FromUtc(2026, 1, 1, 0, 0)));
        var actual = await sut.ExecuteAsync(CancellationToken.None);

        actual
            .Should()
            .BeEquivalentTo([
                new TeActiverenErkenning(VCode.Create("V0001001"), 1),
                new TeActiverenErkenning(VCode.Create("V0001001"), 2),
            ]);
    }

    private static async Task InsertVerenigingen(IDocumentSession session)
    {
        session.Store(
            Vereniging(
                "V0001001",
                false,
                VerenigingStatus.StatusActief.Naam,
                [
                    Erkenning(1, Today, Today.AddDays(10), DomainErkenningStatus.InAanvraag.Value),
                    Erkenning(2, Today.AddDays(-1), Today.AddDays(10), DomainErkenningStatus.InAanvraag.Value),
                    Erkenning(3, Today.AddDays(1), Today.AddDays(10), DomainErkenningStatus.InAanvraag.Value),
                    Erkenning(4, Today.AddDays(-1), Today.AddDays(10), DomainErkenningStatus.Actief.Value),
                    Erkenning(5, Today.AddDays(-1), Today.AddDays(10), DomainErkenningStatus.Geschorst.Value),
                    Erkenning(6, Today.AddDays(-10), Today.AddDays(-1), DomainErkenningStatus.InAanvraag.Value),
                ]
            ),
            Vereniging(
                "V0001002",
                true,
                VerenigingStatus.StatusActief.Naam,
                [Erkenning(7, Today, Today.AddDays(10), DomainErkenningStatus.InAanvraag.Value)]
            ),
            Vereniging(
                "V0001003",
                false,
                "Dubbel",
                [Erkenning(8, Today, Today.AddDays(10), DomainErkenningStatus.InAanvraag.Value)]
            )
        );

        session.Delete<BeheerVerenigingDetailDocument>("V0001002");

        await session.SaveChangesAsync();
    }

    private static BeheerVerenigingDetailDocument Vereniging(
        string vCode,
        bool deleted,
        string status,
        Erkenning[] erkenningen
    ) =>
        new()
        {
            VCode = vCode,
            Deleted = deleted,
            Status = status,
            Erkenningen = erkenningen,
        };

    private static Erkenning Erkenning(int id, DateOnly startdatum, DateOnly? einddatum, string status) =>
        new()
        {
            ErkenningId = id,
            Startdatum = startdatum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Einddatum = einddatum?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            RedenSchorsing = string.Empty,
            Status = status,
        };

    private sealed class FakeClock : IClock
    {
        private readonly Instant _currentInstant;

        public FakeClock(Instant currentInstant)
        {
            _currentInstant = currentInstant;
        }

        public Instant GetCurrentInstant() => _currentInstant;
    }
}
