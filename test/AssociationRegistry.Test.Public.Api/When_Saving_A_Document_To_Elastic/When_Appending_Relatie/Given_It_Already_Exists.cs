namespace AssociationRegistry.Test.Public.Api.When_Saving_A_Document_To_Elastic.When_Appending_Relatie;

using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_It_Already_Exists
{
    private const string DochterVCode = "V0005998";
    private readonly GivenEventsFixture _fixture;
    private readonly Fixture _autofixture;

    public Given_It_Already_Exists(GivenEventsFixture fixture)
    {
        _fixture = fixture;
        _autofixture = new Fixture().CustomizePublicApi();
    }

    [Fact]
    public async ValueTask Then_It_Does_Not_Add_It_Again()
    {
        // dochter
        var relatie = new VerenigingZoekDocument.Types.Relatie
        {
            AndereVereniging = _autofixture.Create<VerenigingZoekDocument.Types.GerelateerdeVereniging>(),
        };

        var repository = new ElasticRepository(_fixture.ElasticClient);

        await _fixture.ElasticClient.IndexDocumentAsync(new VerenigingZoekDocument
        {
            VCode = DochterVCode,
            Relaties = new[]
            {
                relatie,
            },
            Sequence = 10
        });

        await repository.AppendRelatie(DochterVCode, relatie, 10);

        var dochter = await _fixture.ElasticClient.GetAsync<VerenigingZoekDocument>(DochterVCode);
        dochter.Source.Relaties.Should().HaveCount(1);
    }
}
