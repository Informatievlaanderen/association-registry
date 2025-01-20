namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Alba;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public class RegistreerVerenigingUitKboRequestFactory : ITestRequestFactory<RegistreerVerenigingUitKboRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public RegistreerVerenigingUitKboRequestFactory()
    {
    }

    public async Task<RequestResult<RegistreerVerenigingUitKboRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = "0451289431",
        };

        var vCode = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/kbo");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();

            s.Header("Location")
             .SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First()!.Split('/').Last();;

        await apiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<RegistreerVerenigingUitKboRequest>(VCode.Create(vCode), request);
    }
}
