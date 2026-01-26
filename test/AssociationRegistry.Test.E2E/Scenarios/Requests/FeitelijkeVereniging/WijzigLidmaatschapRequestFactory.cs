namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Primitives;
using System.Net;
using Vereniging;

public class WijzigLidmaatschapRequestFactory : ITestRequestFactory<WijzigLidmaatschapRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

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

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{vCode}/lidmaatschappen/{lidmaatschapLidmaatschapId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigLidmaatschapRequest>(VCode.Create(vCode), request, sequence);
    }
}
