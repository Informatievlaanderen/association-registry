namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van;

using Events;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;
using IEvent = JasperFx.Events.IEvent;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class CorrigeerMarkeringAlsDubbelVanCollection : ICollectionFixture<CorrigeerMarkeringAlsDubbelVanContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class CorrigeerMarkeringAlsDubbelVanContext : TestContextBase<VerenigingWerdGemarkeerdAlsDubbelVanScenario, NullRequest>
{
    public IEvent? AanvaarddeCorrectieDubbeleVereniging { get; private set; }

    protected override VerenigingWerdGemarkeerdAlsDubbelVanScenario InitializeScenario()
        => new();

    public CorrigeerMarkeringAlsDubbelVanContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingWerdGemarkeerdAlsDubbelVanScenario scenario)
    {
        CommandResult = await new CorrigeerMarkeringAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);

        using var scope = ApiSetup.AdminApiHost.Services.CreateScope();
        await using var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var stream = await session
                          .Events.FetchStreamAsync(scenario.AuthentiekeVereniging.VCode);
        var counter = 0;

        AanvaarddeCorrectieDubbeleVereniging = stream
           .SingleOrDefault(x => x.EventType == typeof(VerenigingAanvaarddeCorrectieDubbeleVereniging));

        while(AanvaarddeCorrectieDubbeleVereniging is null && counter < 100)

        {
            counter++;
            await Task.Delay(500 + (counter * 100));

            stream = await session.Events.FetchStreamAsync(scenario.AuthentiekeVereniging.VCode);

            AanvaarddeCorrectieDubbeleVereniging = stream
               .SingleOrDefault(x => x.EventType == typeof(VerenigingAanvaarddeCorrectieDubbeleVereniging));
        }

        AanvaarddeCorrectieDubbeleVereniging.Should().NotBeNull();
    }
}
