namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Alba;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using System.Net;

public class WijzigBasisgegevensRequestFactory : ITestRequestFactory<WijzigBasisgegevensRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly IFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public WijzigBasisgegevensRequestFactory(IFeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigBasisgegevensRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigBasisgegevensRequest()
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = NullOrEmpty<DateOnly>.Create(DateOnly.FromDateTime(DateTime.Today)),
            IsUitgeschrevenUitPubliekeDatastroom = !_scenario.FeitelijkeVerenigingWerdGeregistreerd.IsUitgeschrevenUitPubliekeDatastroom,
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            HoofdactiviteitenVerenigingsloket = ["BIAG", "BWWC"],
            Werkingsgebieden = ["BE21"],
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigBasisgegevensRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request, sequence);
    }
}
