namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using AssociationRegistry.Integrations.Grar.Clients;
using Moq;

public class MockGrarClientBuilder
{
    private readonly Fixture _fixture;
    private MockScore[] _responses;

    public MockGrarClientBuilder(Fixture fixture)
    {
        _fixture = fixture;
    }

    public MockGrarClientBuilder WithResponses(params MockScore[] responses)
    {
        _responses = responses;
        return this;
    }

    public MockGrarClientBuilder WithNoResponses()
    {
        _responses = [];

        return this;
    }

    public Mock<IGrarClient> Build()
    {
        var grarClient = new Mock<IGrarClient>();

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new AdresMatchResponseCollection(
                                    _responses.Select(x => _fixture.Create<AddressMatchResponse>() with
                                    {
                                        Score = x.Score,
                                        AdresId = x.AdresId ?? _fixture.Create<Registratiedata.AdresId>(),
                                    }).ToArray()
                                ));

        return grarClient;
    }
}

public record MockScore(double Score, Registratiedata.AdresId? AdresId = null);
