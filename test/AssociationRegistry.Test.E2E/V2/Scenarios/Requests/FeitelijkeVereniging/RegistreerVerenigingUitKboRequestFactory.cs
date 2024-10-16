namespace AssociationRegistry.Test.E2E.V2.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Hosts.Configuration.ConfigurationBindings;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Vereniging;
using Adres = Admin.Api.Verenigingen.Common.Adres;

public class RegistreerVerenigingUitKboRequestFactory : ITestKboRequestFactory<RegistreerVerenigingUitKboRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public RegistreerVerenigingUitKboRequestFactory()
    {
    }

    public async Task<RequestKboResult<RegistreerVerenigingUitKboRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = "0442528054",
        };


        var kboNummer = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/kbo");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();

            s.Header("Location")
             .SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            // s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            // s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First()!.Split('/').Last();

        await apiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestKboResult<RegistreerVerenigingUitKboRequest>(KboNummer.Create(kboNummer), request);
    }
}
