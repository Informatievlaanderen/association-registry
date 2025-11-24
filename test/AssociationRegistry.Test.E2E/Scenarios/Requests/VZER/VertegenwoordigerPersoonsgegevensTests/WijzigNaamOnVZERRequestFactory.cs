namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER.VertegenwoordigerPersoonsgegevensTests;

using Alba;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;
using AutoFixture;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using System.Net;

public class WijzigNaamOnVZERRequestFactory : ITestRequestFactory<WijzigBasisgegevensRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VertegenwoordigerPersoonsgegevensOnVZERScenario _scenario;

    public WijzigNaamOnVZERRequestFactory(VertegenwoordigerPersoonsgegevensOnVZERScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigBasisgegevensRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigBasisgegevensRequest()
        {
            Naam = autoFixture.Create<string>(),
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigBasisgegevensRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
