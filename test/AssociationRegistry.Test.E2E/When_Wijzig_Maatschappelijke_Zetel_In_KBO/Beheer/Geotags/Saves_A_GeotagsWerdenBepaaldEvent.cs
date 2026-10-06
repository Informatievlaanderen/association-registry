namespace AssociationRegistry.Test.E2E.When_Wijzig_Maatschappelijke_Zetel_In_KBO.Beheer.Geotags;

using Events;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

[Collection(nameof(WijzigMaatschappleijkeZetelInKBOCollection))]
public class Saves_A_GeotagsWerdenBepaaldEvent : End2EndTest<IReadOnlyList<GeotagsWerdenBepaald>>
{
    private readonly WijzigMaatschappleijkeZetelInKBOContext _testContext;

    public Saves_A_GeotagsWerdenBepaaldEvent(WijzigMaatschappleijkeZetelInKBOContext testContext)
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

        while (events.Count == 0 && counter < 200)
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
        Response.Should().NotBeEmpty();

        // geotags are idempotent, so we only expect one event.
        Response
            .Single()
            .Geotags.Select(x => x.Identificiatie)
            .Should()
            .Contain(_testContext.Scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode);
    }
}
