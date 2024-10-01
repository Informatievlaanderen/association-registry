namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen;
using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Framework.TestClasses;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Scenarios;
using Scenarios.Commands;
using System.Net;
using Vereniging;
using Xunit;
using Adres = Admin.Api.Verenigingen.Common.Adres;
public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _verenigingWerdGeregistreerdScenario;
    public RegistreerFeitelijkeVerenigingRequest Request => RequestResult.Request;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _verenigingWerdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
    }

    public async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_verenigingWerdGeregistreerdScenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(
            _verenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<RegistreerFeitelijkeVerenigingRequest> RequestResult { get; set; }

    public async Task DisposeAsync()
    {

    }
}
