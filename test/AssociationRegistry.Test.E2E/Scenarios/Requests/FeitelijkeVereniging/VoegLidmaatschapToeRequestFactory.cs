namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Marten.Events;
using System.Net;
using Vereniging;

public class VoegLidmaatschapToeRequestFactory : ITestRequestFactory<VoegLidmaatschapToeRequest>
{
    private readonly IFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public VoegLidmaatschapToeRequestFactory(IFeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<VoegLidmaatschapToeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var date = fixture.Create<DateOnly>();
        var request = new VoegLidmaatschapToeRequest
        {
            AndereVereniging = _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            Van = date,
            Tot = date.AddDays(new Random().Next(1, 99)),
            Identificatie = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/lidmaatschappen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<VoegLidmaatschapToeRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request);
    }
}
