namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.AlbaHost;
using Newtonsoft.Json;
using System.Net;
using When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_A_Expected_Sequence : IClassFixture<BeheerDetailContext>
{
    private readonly BeheerDetailContext _testContext;

    public Given_A_Expected_Sequence(BeheerDetailContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task With_A_Sequence_Equal_Or_Greater_Than_The_Expected_Sequence_Then_we_get_a_successful_response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetailWithHeader(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.RequestResult.VCode,
                                                                        _testContext.RequestResult.Sequence!.Value)
                                                                                    .Should().NotBeNull();
    [Fact]
    public async Task With_A_Sequence_Less_Than_The_Expected_Sequence_Then_We_Get_A_PreconditionFailed_Response()
    {
        var response = _testContext.ApiSetup.AdminApiHost
                                   .GetBeheerDetailHttpResponse(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.RequestResult.VCode,
                                                    long.MaxValue);

        response
           .StatusCode
           .Should().Be(HttpStatusCode.PreconditionFailed);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        responseContentObject!.Title.Should().Be("Er heeft zich een fout voorgedaan!");
        responseContentObject.ProblemTypeUri.Should().Be("urn:associationregistry.admin.api:validation");
    }
}
