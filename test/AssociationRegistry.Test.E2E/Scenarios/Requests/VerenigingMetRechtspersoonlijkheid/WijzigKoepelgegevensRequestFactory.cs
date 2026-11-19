namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

public class WijzigKoepelgegevensRequestFactory : ITestRequestFactory<WijzigKoepelgegevensRequest>
{
    private readonly IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public WijzigKoepelgegevensRequestFactory(IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigKoepelgegevensRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigKoepelgegevensRequest
        {
            Koepelstatus = DomainKoepelstatus.Koepel.Value,
            PrivacyVerklaringGeaccepteerd = true,
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Patch.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}/koepelgegevens"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigKoepelgegevensRequest>(
            VCode.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
