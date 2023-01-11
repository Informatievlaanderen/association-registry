namespace AssociationRegistry.Test.Public.Api.When_retrieving_a_detail_of_a_vereniging;

using AutoFixture;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vereniging;
using Xunit;

public class Given_Two_Verenigingen_With_Minimal_Data_Fixture : PublicApiFixture
{
    private static readonly Fixture Fixture = new();

    public static readonly IDictionary<string, VerenigingWerdGeregistreerd> Events = new Dictionary<string, VerenigingWerdGeregistreerd>()
    {
        { VCode1, VerenigingWerdGeregistreerd(VCode1)},
        { VCode2, VerenigingWerdGeregistreerd(VCode2) },
    };

    private static VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode)
        => new(vCode, Fixture.Create<string>(), null, null, null, null, Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(), Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            DateOnly.FromDateTime(DateTime.Today));

    public const string VCode1 = "v000001";
    public const string VCode2 = "v000002";

    public Given_Two_Verenigingen_With_Minimal_Data_Fixture() : base(nameof(Given_Two_Verenigingen_With_Minimal_Data_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AddEvent(VCode1, Events[VCode1]);
        await AddEvent(VCode2, Events[VCode2]);
    }
}

public class Given_Two_Verenigingen_With_Minimal_Data : IClassFixture<Given_Two_Verenigingen_With_Minimal_Data_Fixture>
{
    private readonly PublicApiClient _publicApiClient;

    public Given_Two_Verenigingen_With_Minimal_Data(Given_Two_Verenigingen_With_Minimal_Data_Fixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData(Given_Two_Verenigingen_With_Minimal_Data_Fixture.VCode1)]
    [InlineData(Given_Two_Verenigingen_With_Minimal_Data_Fixture.VCode2)]
    public async Task Then_we_get_a_detail_vereniging_response(string vCode)
    {
        var response = await _publicApiClient.GetDetail(vCode);

        var content = await response.Content.ReadAsStringAsync();

        var jObject = JsonConvert.DeserializeObject<JObject>(content)!;

        jObject.SelectToken("vereniging.vCode")!.Value<string>().Should().Be(vCode);
        jObject.SelectToken("vereniging.naam")!.Value<string>().Should()
            .Be(Given_Two_Verenigingen_With_Minimal_Data_Fixture.Events[vCode].Naam);
    }
}
