namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;

using Admin.Api.DecentraalBeheer.Verenigingen.Common;
using Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Alba;
using Admin.Api.Infrastructure;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using Marten.Events;
using System.Net;

public class WijzigBasisgegevensRequestFactory : ITestRequestFactory<WijzigBasisgegevensRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";
    private readonly IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public WijzigBasisgegevensRequestFactory(IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<WijzigBasisgegevensRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigBasisgegevensRequest
        {
            KorteBeschrijving = autoFixture.Create<string>(),
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            HoofdactiviteitenVerenigingsloket = ["BIAG", "BWWC"],
            Werkingsgebieden = ["BE25"],
            Roepnaam = "Roep!",
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}/kbo");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        });

        await apiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigBasisgegevensRequest>(
            VCode.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode), request);
    }
}
