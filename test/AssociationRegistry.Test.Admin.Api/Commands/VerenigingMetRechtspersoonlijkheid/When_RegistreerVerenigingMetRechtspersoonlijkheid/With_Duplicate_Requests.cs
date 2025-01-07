namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests
{
    private static When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests? called;
    public readonly RegistreerVerenigingUitKboRequest UitKboRequest;
    public readonly List<Tuple<Task<HttpResponseMessage>, Task<HttpResponseMessage>>> Responses;

    private When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests(EventsInDbScenariosFixture fixture)
    {
        var autofixture = new Fixture().CustomizeAdminApi();

        UitKboRequest = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = fixture.V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection
                               .VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
        };

        Responses = new List<Tuple<Task<HttpResponseMessage>, Task<HttpResponseMessage>>>();

        foreach (var kboNummer in autofixture.CreateMany<KboNummer>(5))
        {
            var request = new RegistreerVerenigingUitKboRequest { KboNummer = kboNummer };

            Responses.Add(new Tuple<Task<HttpResponseMessage>, Task<HttpResponseMessage>>(
                              fixture.DefaultClient.RegistreerKboVereniging(GetJsonBody(request)),
                              fixture.DefaultClient.RegistreerKboVereniging(GetJsonBody(request))));
        }
    }

    public static When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests(fixture);

    private string GetJsonBody(RegistreerVerenigingUitKboRequest uitKboRequest)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_kboNummer")
          .Replace(oldValue: "{{kboNummer}}", uitKboRequest.KboNummer);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Duplicate_Requests
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Duplicate_Requests(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerVerenigingUitKboRequest Request
        => When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests.Called(_fixture).UitKboRequest;

    [Fact]
    public async Task Then_it_returns_an_ok_response_with_correct_headers()
    {
        var responses = When_RegistreerVerenigingMetRechtspersoonlijkheid_With_Duplicate_Requests.Called(_fixture).Responses;

        foreach (var responseTuple in responses)
        {
            var httpResponseMessages = await Task.WhenAll(responseTuple.Item1, responseTuple.Item2);
            httpResponseMessages.Count(x => x.StatusCode == HttpStatusCode.Accepted).Should().Be(1);
        }
    }
}
