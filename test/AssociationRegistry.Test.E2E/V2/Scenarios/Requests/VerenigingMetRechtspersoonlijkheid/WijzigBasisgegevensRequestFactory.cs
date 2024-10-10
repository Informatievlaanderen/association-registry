namespace AssociationRegistry.Test.E2E.V2.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;

using Alba;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Vereniging;
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
            Werkingsgebieden = ["BE"],
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
