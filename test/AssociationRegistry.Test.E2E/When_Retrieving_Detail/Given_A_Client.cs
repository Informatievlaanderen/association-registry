namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using Admin.Api;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_A_Client :
    End2EndTest<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest,
        DetailVerenigingResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Given_A_Client(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext)
    {
        TestContext = _testContext = testContext;
    }

    [Fact]
    public async ValueTask With_An_UnauthorizedClient_Then_We_Get_Unauthorized_Response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetailHttpResponse(_testContext.ApiSetup.UnauthorizedClient, _testContext.CommandResult.VCode,
                                                              _testContext.CommandResult.Sequence!.Value)
                       .StatusCode
                       .Should().Be(HttpStatusCode.Forbidden);
    [Fact]
    public async ValueTask With_An_UnAuthenticatedClient_Then_We_Get_Forbidden_Response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetailHttpResponse(_testContext.ApiSetup.UnautenticatedClient, _testContext.CommandResult.VCode,
                                                              _testContext.CommandResult.Sequence!.Value)
                       .StatusCode
                       .Should().Be(HttpStatusCode.Unauthorized);

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
    {
        get
        {
            return setup =>
            {
                var logger = setup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("EXECUTING GET REQUEST");

                return setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, TestContext.CommandResult.VCode,
                                                                    TestContext.CommandResult.Sequence)
                            .GetAwaiter().GetResult();
            };
        }
    }
}
