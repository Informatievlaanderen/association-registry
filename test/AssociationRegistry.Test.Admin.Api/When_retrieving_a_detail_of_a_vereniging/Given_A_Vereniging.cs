namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Framework;
using VCodes;
using Xunit;

public class Given_A_Vereniging_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    private readonly string _vCode;

    public Given_A_Vereniging_Fixture() : base(nameof(Given_A_Vereniging_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        _vCode = _fixture.Create<VCode>();
    }

    public long Sequence { get; private set; }

    public override async Task InitializeAsync()
    {
        var verenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = _vCode };
        var metadata = _fixture.Create<CommandMetadata>();
        Sequence = await AddEvent(
            _vCode,
            verenigingWerdGeregistreerd,
            metadata);
    }
}

public class Given_A_Vereniging : IClassFixture<Given_A_Vereniging_Fixture>
{
    private const string _vCode = "v000001";
    private readonly Given_A_Vereniging_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Vereniging(Given_A_Vereniging_Fixture fixture)
    {
        _fixture = fixture;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _adminApiClient.GetDetail(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Vereniging)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
