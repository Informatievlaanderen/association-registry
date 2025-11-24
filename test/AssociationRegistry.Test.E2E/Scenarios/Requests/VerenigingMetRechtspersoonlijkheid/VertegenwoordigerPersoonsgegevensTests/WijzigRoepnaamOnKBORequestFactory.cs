namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevensTests;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.MetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using System.Net;

public class WijzigRoepnaamOnKBORequestFactory : ITestRequestFactory<WijzigBasisgegevensRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VertegenwoordigerPersoonsgegevensOnKBOScenario _scenario;

    public WijzigRoepnaamOnKBORequestFactory(VertegenwoordigerPersoonsgegevensOnKBOScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigBasisgegevensRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigBasisgegevensRequest()
        {
            Roepnaam = autoFixture.Create<string>(),
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}/kbo");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigBasisgegevensRequest>(VCode.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
