namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Events;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;
using IEvent = JasperFx.Events.IEvent;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(MarkeerAlsDubbelVanCollection))]
public class MarkeerAlsDubbelVanCollection : ICollectionFixture<MarkeerAlsDubbelVanContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class MarkeerAlsDubbelVanContext : TestContextBase<MultipleWerdGeregistreerdScenario, MarkeerAlsDubbelVanRequest>
{
    public IEvent? VerenigingAanvaarddeDubbeleVereniging { get; private set; }

    protected override MultipleWerdGeregistreerdScenario InitializeScenario()
        => new MultipleWerdGeregistreerdScenario();

    public MarkeerAlsDubbelVanContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(MultipleWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new MarkeerAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);

        using var scope = ApiSetup.AdminApiHost.Services.CreateScope();
        await using var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var stream = await session
                          .Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
        var counter = 0;

        VerenigingAanvaarddeDubbeleVereniging = stream
           .SingleOrDefault(x => x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging));

        while(VerenigingAanvaarddeDubbeleVereniging is null && counter < 100)

        {
            counter++;
            await Task.Delay(500);

            stream = await session.Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);

            VerenigingAanvaarddeDubbeleVereniging = stream
               .SingleOrDefault(x => x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging));
        }

        VerenigingAanvaarddeDubbeleVereniging.Should().NotBeNull();
    }
}
