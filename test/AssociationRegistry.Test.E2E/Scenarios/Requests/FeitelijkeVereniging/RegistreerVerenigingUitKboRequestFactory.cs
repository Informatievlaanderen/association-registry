namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Alba;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public class RegistreerVerenigingUitKboRequestFactory : ITestRequestFactory<RegistreerVerenigingUitKboRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public RegistreerVerenigingUitKboRequestFactory()
    {
    }

    public async Task<CommandResult<RegistreerVerenigingUitKboRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = "0451289431",
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.IgnoreStatusCode();
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/kbo");

            s.Header("Location").ShouldHaveValues();

            s.Header("Location")
             .SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
        })).Context.Response;

        response.StatusCode.Should().BeOneOf((int)HttpStatusCode.OK, (int)HttpStatusCode.Accepted);
        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        var vCode = response.Headers.Location.First()!.Split('/').Last();;

        return new CommandResult<RegistreerVerenigingUitKboRequest>(VCode.Create(vCode), request, sequence);
    }
}
