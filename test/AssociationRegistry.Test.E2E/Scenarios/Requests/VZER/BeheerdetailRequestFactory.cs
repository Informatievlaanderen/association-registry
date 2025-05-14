namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Alba;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public class BeheerdetailRequestFactory : ITestRequestFactory<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public BeheerdetailRequestFactory()
    {
    }

    public async Task<CommandResult<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = autoFixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/vzer");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();

            s.Header("Location")
             .SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        var vCode = response.Headers.Location.First().Split('/').Last();
        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        await apiSetup.AdminProjectionDaemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));

        return new CommandResult<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(VCode.Create(vCode), request, sequence);
    }
}
