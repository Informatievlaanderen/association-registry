namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van;

using Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Common.DubbelMerge;
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
    public IEvent? DubbeleVerenigingWerdSamengevoegd { get; private set; }

    /// <summary>De dubbele vereniging heeft alle basisgegevens, de authentieke enkel een korte naam.</summary>
    public static readonly TestBasisgegevens BasisgegevensDubbeleVereniging = TestBasisgegevens.Fellowship;

    public static readonly TestBasisgegevens BasisgegevensAuthentiekeVereniging = TestBasisgegevens.Empty with
    {
        KorteNaam = TestBasisgegevens.Rivendell.KorteNaam,
    };

    protected override MultipleWerdGeregistreerdScenario InitializeScenario() =>
        new(BasisgegevensDubbeleVereniging, BasisgegevensAuthentiekeVereniging);

    public MarkeerAlsDubbelVanContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(MultipleWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new MarkeerAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);

        using var scope = ApiSetup.AdminApiHost.Services.CreateScope();
        await using var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var stream = await session.Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
        var counter = 0;

        VerenigingAanvaarddeDubbeleVereniging = stream.SingleOrDefault(x =>
            x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging)
        );

        while (VerenigingAanvaarddeDubbeleVereniging is null && counter < 200)
        {
            counter++;
            await Task.Delay(500);

            stream = await session.Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);

            VerenigingAanvaarddeDubbeleVereniging = stream.SingleOrDefault(x =>
                x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging)
            );
        }

        VerenigingAanvaarddeDubbeleVereniging.Should().NotBeNull();

#pragma warning disable CS0618
        DubbeleVerenigingWerdSamengevoegd = stream.SingleOrDefault(x =>
            x.Data is DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging
        );
#pragma warning restore CS0618

        DubbeleVerenigingWerdSamengevoegd.Should().NotBeNull();
    }
}
