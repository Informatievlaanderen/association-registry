namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.DecentraalBeheer.Verenigingen;
using Admin.Api.DecentraalBeheer.Verenigingen.Common;
using Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Alba;
using Admin.Api.Infrastructure;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Adres = Admin.Api.DecentraalBeheer.Verenigingen.Common.Adres;

public class RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamFactory : ITestRequestFactory<RegistreerFeitelijkeVerenigingRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamFactory()
    {
    }

    public async Task<RequestResult<RegistreerFeitelijkeVerenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties = autoFixture.CreateMany<ToeTeVoegenLocatie>().ToArray();
        request.Naam = "Ryugo Kortrijk";
        request.Locaties[0].Adres.Postcode = "8500";
        request.Locaties[0].Adres.Gemeente = "Kortrijk";


        var bevestigingsTokenHelper = new BevestigingsTokenHelper(apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>());

        var hashForAllowingDuplicate = bevestigingsTokenHelper.Calculate(request);

        var vCode = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader(WellknownHeaderNames.BevestigingsToken, hashForAllowingDuplicate);
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();
            s.Header("Location").SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First().Split('/').Last();

        await apiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<RegistreerFeitelijkeVerenigingRequest>(VCode.Create(vCode), request);
    }
}
