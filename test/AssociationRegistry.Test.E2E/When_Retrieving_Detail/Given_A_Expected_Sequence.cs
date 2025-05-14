namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.AlbaHost;
using System.Net;
using Xunit;

[Collection(nameof(BeheerDetailCollection))]
public class Given_A_Expected_Sequence : IClassFixture<BeheerDetailContext>
{
    private readonly BeheerDetailContext _testContext;

    public Given_A_Expected_Sequence(BeheerDetailContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask With_An_Expected_Sequence_Less_than_The_Actual_Then_we_get_a_successful_response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetail(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.CommandResult.VCode,
                                                              new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence - 1))
                                                                                    .Should().NotBeNull();

    [Fact]
    public async ValueTask With_An_Expected_Sequence_Equal_To_The_Actual_Then_we_get_a_successful_response()
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetail(_testContext.ApiSetup.SuperAdminHttpClient, _testContext.CommandResult.VCode,
                                                              new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence))
                                                                                    .Should().NotBeNull();
    [Fact]
    public async ValueTask With_An_Expected_Sequence_More_than_The_Actual_Then_we_get_a_successful_response()
    {
        var response = await SmartHttpClient
                      .Create(_testContext.ApiSetup.AdminApiHost, _testContext.ApiSetup.SuperAdminHttpClient,
                              new RequestParameters().WithExpectedSequence(long.MaxValue))
                      .GetAsync<ProblemDetails>($"/v1/verenigingen/{_testContext.CommandResult.VCode}");

        response.Should().NotBeNull();

        response
           .HttpStatus
           .Should().Be((int)HttpStatusCode.PreconditionFailed);

        response.Title.Should().Be("Er heeft zich een fout voorgedaan!");
        response.ProblemTypeUri.Should().Be("urn:associationregistry.admin.api:validation");
    }
}
