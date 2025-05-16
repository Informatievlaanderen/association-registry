namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using Primitives;
using System.Net;
using Vereniging;

public class WijzigLidmaatschapRequestFactory : ITestRequestFactory<WijzigLidmaatschapRequest>
{
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;

    public WijzigLidmaatschapRequestFactory(LidmaatschapWerdToegevoegdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigLidmaatschapRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.LidmaatschapWerdToegevoegd.VCode;
        var lidmaatschapLidmaatschapId = _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId;

        var fixture = new Fixture().CustomizeAdminApi();

        var date = fixture.Create<DateOnly>();

        var request = new WijzigLidmaatschapRequest()
        {
            Van = NullOrEmpty<DateOnly>.Create(date),
            Tot = NullOrEmpty<DateOnly>.Create(date.AddDays(new Random().Next(1, 99))),
            Identificatie = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{vCode}/lidmaatschappen/{lidmaatschapLidmaatschapId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new CommandResult<WijzigLidmaatschapRequest>(VCode.Create(vCode), request);
    }
}
