namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingOfAnyKind;

using Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using System.Net;
using Vereniging;

public class MaakSubverenigingRequestFactory : ITestRequestFactory<MaakSubverenigingRequest>
{
    private readonly MultipleWerdGeregistreerdScenario _scenario;

    public MaakSubverenigingRequestFactory(MultipleWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<MaakSubverenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var date = fixture.Create<DateOnly>();
        var request = new MaakSubverenigingRequest
        {
            AndereVereniging = _scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            Identificatie = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/subverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<MaakSubverenigingRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request);
    }
}
