namespace AssociationRegistry.Test.Public.Api.When_Saving_A_Document_To_Elastic;

using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Xunit;
using Xunit.Categories;

public class Given_A_Valid_VerenigingDocument_Fixture : ElasticRepositoryFixture
{
    public Given_A_Valid_VerenigingDocument_Fixture() : base(nameof(Given_A_Valid_VerenigingDocument_Fixture))
    {
    }
}

[IntegrationTest]
public class Given_A_Valid_VerenigingDocument : IClassFixture<Given_A_Valid_VerenigingDocument_Fixture>
{
    private readonly Given_A_Valid_VerenigingDocument_Fixture _classFixture;

    public Given_A_Valid_VerenigingDocument(Given_A_Valid_VerenigingDocument_Fixture classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public void Then_it_does_not_throw_an_exception()
    {
        var fixture = new Fixture();

        _classFixture.ElasticRepository!
                     .Index(
                          new VerenigingZoekDocument
                          {
                              VCode = fixture.Create<string>(),
                              Naam = fixture.Create<string>(),
                              KorteNaam = fixture.Create<string>(),
                              Locaties = new[] { fixture.Create<VerenigingZoekDocument.Types.Locatie>() },
                              HoofdactiviteitenVerenigingsloket = new[]
                                  { fixture.Create<VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket>() },
                          });
    }
}
