namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using AutoFixture;
using Fixtures;
using FluentAssertions;
using Xunit;
using Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture : PublicApiFixture
{
    private static readonly Fixture Fixture = new();

    public static readonly IDictionary<string, VerenigingWerdGeregistreerd> Events = new Dictionary<string, VerenigingWerdGeregistreerd>()
    {
        { VCode1, new VerenigingWerdGeregistreerd(VCode1, Fixture.Create<string>()) },
        { VCode2, new VerenigingWerdGeregistreerd(VCode2, Fixture.Create<string>()) },
    };

    public const string VCode1 = "v000001";
    public const string VCode2 = "v000002";

    public Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture() : base(nameof(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AddEvent(VCode1, Events[VCode1]);
        await AddEvent(VCode2, Events[VCode2]);
    }
}

public class Given_Multiple_Verenigingen_With_Actual_Minimal_Data : IClassFixture<Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture>
{
    private readonly HttpClient _httpClient;

    public Given_Multiple_Verenigingen_With_Actual_Minimal_Data(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Theory]
    [InlineData(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture.VCode1)]
    [InlineData(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture.VCode2)]
    public async Task Then_we_get_a_detail_vereniging_response(string vCode)
    {
        var responseMessage = await _httpClient.GetAsync($"/v1/verenigingen/{vCode}");

        var content = await responseMessage.Content.ReadAsStringAsync();

        var jObject = JsonConvert.DeserializeObject<JObject>(content)!;

        jObject.SelectToken("vereniging.id")!.Value<string>().Should().Be(vCode);
        jObject.SelectToken("vereniging.naam")!.Value<string>().Should()
            .Be(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture.Events[vCode].Naam);
    }
}
