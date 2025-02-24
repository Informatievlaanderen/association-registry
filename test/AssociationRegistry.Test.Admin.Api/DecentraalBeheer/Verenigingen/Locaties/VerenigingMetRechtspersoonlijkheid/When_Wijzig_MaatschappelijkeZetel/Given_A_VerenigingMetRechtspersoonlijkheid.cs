namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigMaatschappelijkeZetel_Setup
{
    public WijzigMaatschappelijkeZetelRequest Request { get; }
    public V043_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForWijzigen Scenario { get; }
    public HttpResponseMessage Response { get; }

    public When_WijzigMaatschappelijkeZetel_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V043VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForWijzigen;

        Request = new Fixture().CustomizeAdminApi().Create<WijzigMaatschappelijkeZetelRequest>();

        var jsonBody = $@"{{
            ""locatie"": {{
                ""naam"":""{Request.Locatie.Naam}"",
                ""isPrimair"":""{Request.Locatie.IsPrimair}"",
                }}
            }}";

        Response = fixture.DefaultClient
                          .PatchMaatschappelijkeZetel(Scenario.VCode, Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                                      jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<When_WijzigMaatschappelijkeZetel_Setup>
{
    private readonly WijzigMaatschappelijkeZetelRequest _request;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;
    private readonly int _locatieId;

    public Given_A_VerenigingMetRechtspersoonlijkheid(When_WijzigMaatschappelijkeZetel_Setup setup, EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _vCode = setup.Scenario.VCode;
        _locatieId = setup.Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId;
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

        var maatschappelijkeZetelVolgensKboWerdGewijzigd =
            events.Single(e => e.Data.GetType() == typeof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd));

        maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Should()
                                                    .BeEquivalentTo(
                                                         new MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
                                                             _locatieId, _request.Locatie.Naam!,
                                                             _request.Locatie.IsPrimair!.Value));
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
