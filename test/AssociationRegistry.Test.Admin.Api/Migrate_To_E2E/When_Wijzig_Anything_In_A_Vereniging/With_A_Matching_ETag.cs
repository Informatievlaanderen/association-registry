namespace AssociationRegistry.Test.Admin.Api.Migrate_To_E2E.When_Wijzig_Anything_In_A_Vereniging;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisgegevens_With_A_Matching_ETag
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;
    public readonly HttpResponseMessage Response;

    private When_WijzigBasisgegevens_With_A_Matching_ETag(EventsInDbScenariosFixture fixture)
    {
        Request = new WijzigBasisgegevensRequest
        {
            Naam = "De nieuwe vereniging",
        };

        VCode = fixture.V005FeitelijkeVerenigingWerdGeregistreerdForUseWithETagMatching.VCode;

        var jsonBody = $@"{{""naam"":""{Request.Naam}""}}";

        var saveVersionResult = fixture.V005FeitelijkeVerenigingWerdGeregistreerdForUseWithETagMatching.Result;
        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody, saveVersionResult.Version).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisgegevens_With_A_Matching_ETag? called;

    public static When_WijzigBasisgegevens_With_A_Matching_ETag Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisgegevens_With_A_Matching_ETag(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[Category(Categories.MoveToBasicE2E)]

[IntegrationTest]
public class With_A_Matching_ETag
{
    private readonly EventsInDbScenariosFixture _fixture;

    private WijzigBasisgegevensRequest Request
        => When_WijzigBasisgegevens_With_A_Matching_ETag.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_WijzigBasisgegevens_With_A_Matching_ETag.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisgegevens_With_A_Matching_ETag.Called(_fixture).VCode;

    public With_A_Matching_ETag(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(HeaderNames.Location);

        Response.Headers.Location!.OriginalString.Should()
                .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        var savedEvents = session.Events
                                 .QueryRawEventDataOnly<NaamWerdGewijzigd>()
                                 .SingleOrDefault(@event => @event.VCode == VCode);

        savedEvents.Should().NotBeNull();
        savedEvents!.Naam.Should().Be(Request.Naam);
    }
}
