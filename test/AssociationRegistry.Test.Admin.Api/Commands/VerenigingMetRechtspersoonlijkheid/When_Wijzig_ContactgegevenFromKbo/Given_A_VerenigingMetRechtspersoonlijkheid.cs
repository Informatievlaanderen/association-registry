namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework;
using Framework.Categories;
using Framework.Fixtures;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_ContactgegevenWerdOvergenomen_Setup
{
    public WijzigContactgegevenRequest Request { get; }
    public V045_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_ContactgegevenFromKbo_For_Wijzigen Scenario { get; }
    public HttpResponseMessage Response { get; }

    public When_ContactgegevenWerdOvergenomen_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V045VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithContactgegevenFromKboForWijzigen;

        Request = new Fixture().CustomizeAdminApi().Create<WijzigContactgegevenRequest>();

        var jsonBody = $@"{{
            ""contactgegeven"":
                {{
                    ""beschrijving"":""{Request.Contactgegeven.Beschrijving}"",
                    ""isPrimair"":""{Request.Contactgegeven.IsPrimair}"",
                }}
            }}";

        Response = fixture.DefaultClient
                          .PatchContactgegevensFromKbo(Scenario.VCode,
                                                       Scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId,
                                                       jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTestToRefactor]
[IntegrationTest]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<When_ContactgegevenWerdOvergenomen_Setup>
{
    private readonly WijzigContactgegevenRequest _request;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;
    private readonly int _contactgegevenId;

    public Given_A_VerenigingMetRechtspersoonlijkheid(When_ContactgegevenWerdOvergenomen_Setup setup, EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _vCode = setup.Scenario.VCode;
        _contactgegevenId = setup.Scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId;
        _documentStore = fixture.DocumentStore;
        _appSettings = fixture.ServiceProvider.GetRequiredService<AppSettings>();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _documentStore
           .LightweightSession();

        var events = session.Events
                            .FetchStream(_vCode);

        var roepnaamWerdGewijzigd = events.Single(e => e.Data.GetType() == typeof(ContactgegevenUitKBOWerdGewijzigd));

        roepnaamWerdGewijzigd.Data.Should()
                             .BeEquivalentTo(
                                  new ContactgegevenUitKBOWerdGewijzigd(_contactgegevenId, _request.Contactgegeven.Beschrijving!,
                                                                        _request.Contactgegeven.IsPrimair!.Value));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _response.Headers.Should().ContainKey(HeaderNames.Location);

        _response.Headers.Location!.OriginalString.Should()
                 .StartWith($"{_appSettings.BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        _response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
