namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using AutoFixture;
using Fixtures;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vereniging;

public class Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture : PublicApiFixture
{
    private static readonly Fixture Fixture = new();

    public static readonly IDictionary<string, VerenigingWerdGeregistreerd> Events = new Dictionary<string, VerenigingWerdGeregistreerd>()
    {
        { VCode1, VerenigingWerdGeregistreerd(VCode1)},
        { VCode2, VerenigingWerdGeregistreerd(VCode2) },
    };

    private static VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode)
        => new(vCode, Fixture.Create<string>(), null, null, null, null, Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(), DateOnly.FromDateTime(DateTime.Today));

    public const string VCode1 = "v000001";
    public const string VCode2 = "v000002";

    public Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture() : base(nameof(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
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

        jObject.SelectToken("vereniging.vCode")!.Value<string>().Should().Be(vCode);
        jObject.SelectToken("vereniging.naam")!.Value<string>().Should()
            .Be(Given_Multiple_Verenigingen_With_Actual_Minimal_Data_Fixture.Events[vCode].Naam);
    }
}
