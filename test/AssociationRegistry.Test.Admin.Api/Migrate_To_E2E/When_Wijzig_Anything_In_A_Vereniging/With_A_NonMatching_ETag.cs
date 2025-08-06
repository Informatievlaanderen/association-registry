namespace AssociationRegistry.Test.Admin.Api.Migrate_To_E2E.When_Wijzig_Anything_In_A_Vereniging;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.ComponentModel;
using System.Net;
using Xunit;

public sealed class When_WijzigBasisgegevens_With_A_NonMatching_ETag
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;
    public readonly HttpResponseMessage Response;

    private When_WijzigBasisgegevens_With_A_NonMatching_ETag(EventsInDbScenariosFixture fixture)
    {
        Request = new WijzigBasisgegevensRequest
        {
            Naam = "De nieuwe vereniging",
        };

        VCode = fixture.V003FeitelijkeVerenigingWerdGeregistreerdForUseWithNoChanges.VCode;

        var jsonBody = $@"{{""naam"":""{Request.Naam}""}}";

        var saveVersionResult = fixture.V003FeitelijkeVerenigingWerdGeregistreerdForUseWithNoChanges.Result;
        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody, saveVersionResult.Version - 1).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisgegevens_With_A_NonMatching_ETag? called;

    public static When_WijzigBasisgegevens_With_A_NonMatching_ETag Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisgegevens_With_A_NonMatching_ETag(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category(Categories.MoveToBasicE2E)]
public class With_A_NonMatching_ETag
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_WijzigBasisgegevens_With_A_NonMatching_ETag.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisgegevens_With_A_NonMatching_ETag.Called(_fixture).VCode;

    public With_A_NonMatching_ETag(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_preconditionfailed_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        Response.Headers.Should().NotContainKey(HeaderNames.Location);
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_saves_no_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        var savedEvents = session.Events
                                 .QueryRawEventDataOnly<NaamWerdGewijzigd>()
                                 .SingleOrDefault(@event => @event.VCode == VCode);

        savedEvents.Should().BeNull();
    }
}
