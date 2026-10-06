namespace AssociationRegistry.Test.E2E.When_Verwijder_Maatschappelijke_Zetel_Uit_KBO.Beheer.Geotags;

using Events;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

[Collection(nameof(VerwijderMaatschappelijkeZetelUitKBOCollection))]
public class Saves_An_Empty_GeotagsWerdenBepaaldEvent : End2EndTest<IReadOnlyList<GeotagsWerdenBepaald>>
{
    private readonly VerwijderMaatschappelijkeZetelUitKBOContext _testContext;

    public Saves_An_Empty_GeotagsWerdenBepaaldEvent(VerwijderMaatschappelijkeZetelUitKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<IReadOnlyList<GeotagsWerdenBepaald>> GetResponse(FullBlownApiSetup setup)
    {
        using var scope = setup.AdminApiHost.Services.CreateScope();
        await using var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var vCode = _testContext.VCode.ToString();

        var events = await QueryEvents(session, vCode);

        var counter = 0;

        // Wait until the second GeotagsWerdenBepaald event (from the removal) is saved,
        // since the first one is caused by the initial MaatschappelijkeZetelWerdOvergenomenUitKbo.
        while (events.Count < 2 && counter < 200)
        {
            counter++;
            await Task.Delay(500);

            events = await QueryEvents(session, vCode);
        }

        return events;
    }

    private static async Task<IReadOnlyList<GeotagsWerdenBepaald>> QueryEvents(
        IDocumentSession session,
        string vCode
    ) => await session.Events.QueryRawEventDataOnly<GeotagsWerdenBepaald>().Where(x => x.VCode == vCode).ToListAsync();

    [Fact]
    public void JsonContentMatches()
    {
        // geotags are idempotent, so we only expect one event.
        Response.Single().Geotags.Should().BeEmpty();
    }
}
