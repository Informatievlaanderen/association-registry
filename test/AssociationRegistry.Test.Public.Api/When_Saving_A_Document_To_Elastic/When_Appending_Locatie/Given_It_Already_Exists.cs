namespace AssociationRegistry.Test.Public.Api.When_Saving_A_Document_To_Elastic.When_Appending_Locatie;

using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using System.Threading.Tasks;
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
    public async Task Then_It_Does_Not_Add_It_Again()
    {
        // dochter
        var locatie = _autofixture.Create<VerenigingZoekDocument.Locatie>();

        var repository = new ElasticRepository(_fixture.ElasticClient);

        await _fixture.ElasticClient.IndexDocumentAsync(new VerenigingZoekDocument
        {
            VCode = DochterVCode,
            Locaties = new[]
            {
                locatie,
            },
        });

        await repository.AppendLocatie(DochterVCode, locatie);

        var dochter = await _fixture.ElasticClient.GetAsync<VerenigingZoekDocument>(DochterVCode);
        dochter.Source.Locaties.Should().HaveCount(1);
    }
}
