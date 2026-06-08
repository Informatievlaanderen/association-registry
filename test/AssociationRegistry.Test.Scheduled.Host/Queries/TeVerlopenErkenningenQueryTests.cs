namespace AssociationRegistry.Test.Scheduled.Host.Queries;

using Admin.Schema.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Scheduled.Host.Queries;
using AssociationRegistry.Test.Common.Framework;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework;
using Marten;
using NodaTime;

public class TeVerlopenErkenningenQueryTests
{
    private static readonly DateOnly Today = new(2026, 1, 1);

    [Fact]
    public async ValueTask Returns_Only_Erkenningen_In_Aanvraag_With_Startdatum_In_The_Past_Or_Today()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeVerlopenErkenningenQueryTests));
        var session = store.LightweightSession();

        await InsertErkenningen(session);

        var sut = new TeVerlopenErkenningenQuery(session, new ClockStub(DateOnly.FromDateTime(DateTime.Today)));
        var actual = await sut.ExecuteAsync(CancellationToken.None);

        actual
           .Should()
           .BeEquivalentTo(
                [
                    Erkenning("V0001001", 5, ErkenningStatus.Actief, Today.AddDays(-1), Today.AddDays(50)),
                    Erkenning("V0001001", 6, ErkenningStatus.Actief, Today.AddDays(0), Today.AddDays(50)),
                    Erkenning("V0001001", 7, ErkenningStatus.Actief, Today.AddDays(-50), Today.AddDays(-10)),
                    Erkenning("V0001001", 8, ErkenningStatus.Actief, Today.AddDays(-10), Today.AddDays(0)),
                    Erkenning("V0001003", 1, ErkenningStatus.Actief, Today.AddDays(-1), Today.AddDays(50)),
                ],
                options => options.ExcludingMissingMembers()
            );
    }

    private static async Task InsertErkenningen(IDocumentSession session)
    {
        session.StoreObjects([
            Erkenning("V0001001", 1, ErkenningStatus.Verlopen, Today.AddDays(-100), Today.AddDays(-50)),
            Erkenning("V0001001", 2, ErkenningStatus.Geschorst, Today.AddDays(-3), Today.AddDays(50)),
            Erkenning("V0001001", 3, ErkenningStatus.InAanvraag, Today.AddDays(-100), Today.AddDays(-50)),
            Erkenning("V0001001", 4, ErkenningStatus.InAanvraag, Today.AddDays(1), Today.AddDays(50)),
            Erkenning("V0001001", 5, ErkenningStatus.Actief, Today.AddDays(-1), Today.AddDays(50)),
            Erkenning("V0001001", 6, ErkenningStatus.Actief, Today.AddDays(0), Today.AddDays(50)),
            Erkenning("V0001001", 7, ErkenningStatus.Actief, Today.AddDays(-50), Today.AddDays(-10)),
            Erkenning("V0001001", 8, ErkenningStatus.Actief, Today.AddDays(-10), Today.AddDays(0)),

            Erkenning("V0001002", 1, ErkenningStatus.InAanvraag, Today, Today.AddDays(50)),
            Erkenning("V0001003", 1, ErkenningStatus.Actief, Today.AddDays(-1), Today.AddDays(50)),
            Erkenning("V0001004", 1, ErkenningStatus.Geschorst, Today.AddDays(-1), Today.AddDays(50)),
        ]);

        await session.SaveChangesAsync();
    }

    private static ErkenningDocument Erkenning(
        string vCode,
        int erkenningId,
        ErkenningStatus status,
        DateOnly startdatum,
        DateOnly einddatum
    ) =>
        new()
        {
            Id = $"{vCode}-{erkenningId}",
            VCode = VCode.Create(vCode),
            ErkenningId = erkenningId,
            Status = status.Value,
            Startdatum = startdatum,
            Einddatum = einddatum,
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
