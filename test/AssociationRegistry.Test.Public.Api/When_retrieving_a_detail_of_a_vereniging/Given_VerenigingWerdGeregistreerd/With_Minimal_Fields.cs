namespace AssociationRegistry.Test.Public.Api.When_retrieving_a_detail_of_a_vereniging.Given_VerenigingWerdGeregistreerd;

using System.Text.RegularExpressions;
using AssociationRegistry.Events;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Test.Public.Api.Fixtures;
using AssociationRegistry.Test.Public.Api.Framework;
using FluentAssertions;
using Xunit;

public class With_Minimal_Fields_Fixture : PublicApiFixture
{
    public const string VCode = "V000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public With_Minimal_Fields_Fixture() : base(nameof(With_Minimal_Fields_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>()));
    }
}

public class With_Minimal_Fields : IClassFixture<With_Minimal_Fields_Fixture>
{
    private const string VCode = "V000001";
    private readonly PublicApiClient _publicApiClient;

    public With_Minimal_Fields(With_Minimal_Fields_Fixture werdGeregistreerdFixture)
    {
        _publicApiClient = werdGeregistreerdFixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(VCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _publicApiClient.GetDetail(VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(With_Minimal_Fields)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
