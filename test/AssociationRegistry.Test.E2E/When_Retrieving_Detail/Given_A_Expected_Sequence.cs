namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_A_Expected_Sequence :
    End2EndTest<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest,
        DetailVerenigingResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Given_A_Expected_Sequence(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task With_A_Sequence_Equal_Or_Greater_Than_The_Expected_Sequence_Then_we_get_a_successful_response()
        => GetResponse.Should().NotBeNull();

    [Fact]
    public async Task With_No_Sequence_Provided_Then_We_Get_A_Successful_Response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetail(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.RequestResult.VCode,
                                                              _testContext.RequestResult.Sequence!.Value)
                       .StatusCode
                       .Should().Be(HttpStatusCode.OK);

    [Fact]
    public async Task With_A_Sequence_Less_Than_The_Expected_Sequence_Then_We_Get_A_PreconditionFailed_Response()
    {
        var response = _testContext.ApiSetup.AdminApiHost
                                   .GetBeheerDetail(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.RequestResult.VCode,
                                                    long.MaxValue);

        response
           .StatusCode
           .Should().Be(HttpStatusCode.PreconditionFailed);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        responseContentObject!.Title.Should().Be("Er heeft zich een fout voorgedaan!");
        responseContentObject.ProblemTypeUri.Should().Be("urn:associationregistry.admin.api:validation");
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
    {
        get
        {
            return setup =>
            {
                var logger = setup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("EXECUTING GET REQUEST");

                return setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                    TestContext.RequestResult.Sequence)
                            .GetAwaiter().GetResult();
            };
        }
    }
}
