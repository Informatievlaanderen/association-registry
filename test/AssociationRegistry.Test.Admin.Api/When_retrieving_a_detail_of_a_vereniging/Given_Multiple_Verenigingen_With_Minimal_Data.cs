namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Xunit;

public class Given_Multiple_Verenigingen_With_Minimal_Data_Fixture : AdminApiFixture
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

    private const string VCode1 = "V000001";
    public const string VCode2 = "V000002";

    private static readonly CommandMetadata CommandMetadata = new(
        Initiator: "Een initiator",
        Tijdstip: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero).ToInstant());

    public Given_Multiple_Verenigingen_With_Minimal_Data_Fixture() : base(nameof(Given_Multiple_Verenigingen_With_Minimal_Data_Fixture))
    {
    }

    public HttpResponseMessage Response { get; private set; } = null!;

    protected override async Task Given()
    {
        await AddEvent(VCode1, Events[VCode1], CommandMetadata);
        await AddEvent(VCode2, Events[VCode2], CommandMetadata);
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetDetail(VCode2.ToUpperInvariant());
    }
}

public class Given_Multiple_Verenigingen_With_Minimal_Data : IClassFixture<Given_Multiple_Verenigingen_With_Minimal_Data_Fixture>
{
    private readonly Given_Multiple_Verenigingen_With_Minimal_Data_Fixture _adminApiFixture;

    public Given_Multiple_Verenigingen_With_Minimal_Data(Given_Multiple_Verenigingen_With_Minimal_Data_Fixture adminApiFixture)
    {
        _adminApiFixture = adminApiFixture;
    }

    [Fact]
    public async Task Then_we_get_the_correct_detail_vereniging_response()
    {
        var content = await _adminApiFixture.Response.Content.ReadAsStringAsync();

        var jObject = JsonConvert.DeserializeObject<JObject>(content)!;

        jObject.SelectToken("vereniging.vCode")!.Value<string>().Should().Be(
            Given_Multiple_Verenigingen_With_Minimal_Data_Fixture.VCode2.ToUpperInvariant());
        jObject.SelectToken("vereniging.naam")!.Value<string>().Should()
            .Be(Given_Multiple_Verenigingen_With_Minimal_Data_Fixture.Events[
                Given_Multiple_Verenigingen_With_Minimal_Data_Fixture.VCode2].Naam);
    }
}
