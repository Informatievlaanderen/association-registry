namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using System.Net;
using Vereniging;

public class VoegLidmaatschapToeRequestFactory : ITestRequestFactory<VoegLidmaatschapToeRequest>
{
    private readonly MultipleWerdGeregistreerdScenario _scenario;

    public VoegLidmaatschapToeRequestFactory(MultipleWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VoegLidmaatschapToeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var date = fixture.Create<DateOnly>();
        var request = new VoegLidmaatschapToeRequest
        {
            AndereVereniging = _scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            Van = date,
            Tot = date.AddDays(new Random().Next(1, 99)),
            Identificatie = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/lidmaatschappen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<VoegLidmaatschapToeRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request, sequence);
    }
}
