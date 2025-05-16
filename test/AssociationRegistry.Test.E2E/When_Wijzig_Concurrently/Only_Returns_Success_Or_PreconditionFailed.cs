namespace AssociationRegistry.Test.E2E.When_Wijzig_Concurrently;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.When_Wijzig_Locatie;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(WijzigConcurrentlyCollection))]
public class Only_Returns_Success_Or_PreconditionFailed : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigConcurrentlyContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Only_Returns_Success_Or_PreconditionFailed(WijzigConcurrentlyContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestHeadersBuilder().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public async ValueTask TestZonderExpectedVersion()
    {
        var client = _testContext.ApiSetup.AdminApiHost.CreateClientWithOptionalHeaders(_testContext.ApiSetup.SuperAdminHttpClient);
        var locatie = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First();
        var tasks = new List<Task>();

        for (var i = 0; i < 100; i++)
        {
            var currentIndex = i; // Capture the current index

            var task = Task.Run(async () =>
            {

                var response = await client.PatchAsync($"/v1/verenigingen/{_testContext.VCode}/locaties/{locatie.LocatieId}",
                                                       new StringContent(
                                                           JsonConvert.SerializeObject(new WijzigLocatieRequest
                                                           {
                                                               Locatie = new TeWijzigenLocatie
                                                               {
                                                                   Naam = Guid.NewGuid().ToString(),
                                                               },
                                                           }), Encoding.UTF8, mediaType: "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    _helper.WriteLine("Wijziging is gelukt");
                }
                else
                {
                    _helper.WriteLine($"Wijziging {currentIndex:000} is mislukt: {response.StatusCode}");
                    response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
                }
            });

            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
    }

    [Fact]
    public async ValueTask TestMetExpectedVersion()
    {
        var client = _testContext.ApiSetup.AdminApiHost.CreateClientWithOptionalHeaders(_testContext.ApiSetup.SuperAdminHttpClient);
        var locatie = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First();
        var tasks = new List<Task>();

        for (var i = 0; i < 100; i++)
        {
            var currentIndex = i; // Capture the current index

            var task = Task.Run(async () =>
            {
                    var vereniging = await client.GetAsync($"/v1/verenigingen/{_testContext.VCode}");
                    var expectedVersion = vereniging.Headers.FirstOrDefault(x => x.Key == "ETag").Value;

                    if (expectedVersion == null)
                        _helper.WriteLine(
                            $"Expected version is null: {vereniging.StatusCode} => {vereniging.Headers.Select(x => $"{x.Key}: {x.Value}")}");

                    expectedVersion.Should().NotBeNull();

                    // Create a new HttpRequestMessage with the custom header
                    var request = new HttpRequestMessage(HttpMethod.Patch,
                                                         $"/v1/verenigingen/{_testContext.VCode}/locaties/{locatie.LocatieId}")
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new WijzigLocatieRequest
                            {
                                Locatie = new TeWijzigenLocatie
                                {
                                    Naam = Guid.NewGuid().ToString(),
                                },
                            }), Encoding.UTF8, mediaType: "application/json"),
                    };

                    // Add the custom header
                    request.Headers.Add(WellknownHeaderNames.IfMatch, expectedVersion);

                    // Send the request and await the response
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        _helper.WriteLine("Wijziging is gelukt");
                    }
                    else
                    {
                        _helper.WriteLine($"Wijziging {currentIndex:000} is mislukt: {response.StatusCode}");
                        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
                    }
            });

            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray(), TestContext.Current.CancellationToken);
    }
}
