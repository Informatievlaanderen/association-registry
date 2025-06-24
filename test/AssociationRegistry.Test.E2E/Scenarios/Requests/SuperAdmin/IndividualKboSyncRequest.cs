namespace AssociationRegistry.Test.E2E.Scenarios.Requests.SuperAdmin;

using Admin.Api.Infrastructure;
using AutoFixture;
using Common.AutoFixture;
using Framework.AlbaHost;
using Framework.ApiSetup;
using System.Net;
using System.Net.Http.Json;
using Vereniging;
using VerenigingMetRechtspersoonlijkheid;

public class IndividualKboSyncRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly SmartHttpClient _smartHttpClient;
    private readonly IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public IndividualKboSyncRequestFactory(
        SmartHttpClient smartHttpClient,
        IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _smartHttpClient = smartHttpClient;
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var response =
            _smartHttpClient.PostAsync(
                $"/v1/verenigingen/kbo/sync/{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}", JsonContent.Create(new object()));

        return new CommandResult<NullRequest>(VCode.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode), new NullRequest());
    }
}
