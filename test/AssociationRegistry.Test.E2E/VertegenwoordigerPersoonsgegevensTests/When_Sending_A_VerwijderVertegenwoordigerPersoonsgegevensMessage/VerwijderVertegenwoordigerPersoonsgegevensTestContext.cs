namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Sending_A_VerwijderVertegenwoordigerPersoonsgegevensMessage;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using AssociationRegistry.Test.E2E.Scenarios.Requests.VZER.VertegenwoordigerPersoonsgegevensTests;
using CommandHandling.Bewaartermijnen.Reacties.VerwijderVertegenwoordigerPersoonsgegevens;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.Extensions;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Wolverine;
using Wolverine.Persistence.Durability;
using Xunit;

[CollectionDefinition(nameof(VerwijderVertegenwoordigerPersoonsgegevensCollection))]
public class VerwijderVertegenwoordigerPersoonsgegevensCollection : ICollectionFixture<VerwijderVertegenwoordigerPersoonsgegevensTestContext>
{
}

public class VerwijderVertegenwoordigerPersoonsgegevensTestContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, NullRequest>
{
    private readonly FullBlownApiSetup _apiSetup;

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VerwijderVertegenwoordigerPersoonsgegevensTestContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
        _apiSetup = apiSetup;
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        var vzer = scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

        CommandResult =
            new CommandResult<NullRequest>(VCode.Hydrate(vzer.VCode),
                                           new NullRequest());

        using var scope = _apiSetup.AdminApiHost.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var verwijderVertegenwoordigerPersoonsgegevensMessage = new VerwijderVertegenwoordigerPersoonsgegevensMessage(
            $"{vzer.VCode}-{vzer.Vertegenwoordigers[0].VertegenwoordigerId}", vzer.VCode, vzer.Vertegenwoordigers[0].VertegenwoordigerId+99999, DateTime.UtcNow.ToInstant(), "");

        await bus.SendAsync(verwijderVertegenwoordigerPersoonsgegevensMessage);
    }
}

