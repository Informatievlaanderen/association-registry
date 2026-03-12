namespace AssociationRegistry.Test.Admin.ExpiredBewaartermijn;

using AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Queries;
using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Marten;
using NodaTime;

public class VerlopenBewaartermijnQueryTests
{
    [Fact]
    public async ValueTask Returns_Only_Gepland_Bewaartermijn_With_Verlopen_Vervaldag()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(VerlopenBewaartermijnQueryTests));
        var session = store.LightweightSession();

        var expected = await InsertBewaartermijnen(session);

        var sut = new VerlopenBewaartermijnQuery(session);
        var actual = await sut.ExecuteAsync(CancellationToken.None);

        actual.Should().BeEquivalentTo(expected);
    }

    private async Task<BewaartermijnDocument[]> InsertBewaartermijnen(IDocumentSession session)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        BewaartermijnDocument[] geplandeBewaartermijnenWithVerlopenVervaldag =
        [
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
        ];

        BewaartermijnDocument[] verlopenBewaartermijnenWithVerlopenVervaldag =
        [
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Verlopen.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Verlopen.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
        ];

        BewaartermijnDocument[] geplandBewaartermijnenWithNietVerlopenVervaldag =
        [
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = NietVerlopenVervaldag(fixture),
            },
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = NietVerlopenVervaldag(fixture),
            },
        ];

        session.InsertObjects(geplandeBewaartermijnenWithVerlopenVervaldag
                             .Union(verlopenBewaartermijnenWithVerlopenVervaldag)
                             .Union(geplandBewaartermijnenWithNietVerlopenVervaldag));

        await session.SaveChangesAsync();

        return geplandeBewaartermijnenWithVerlopenVervaldag;
    }

    private static Instant VerlopenVervaldag(Fixture fixture)
        => SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(fixture.Create<int>()));

    private static Instant NietVerlopenVervaldag(Fixture fixture)
        => SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(fixture.Create<int>()));
}
