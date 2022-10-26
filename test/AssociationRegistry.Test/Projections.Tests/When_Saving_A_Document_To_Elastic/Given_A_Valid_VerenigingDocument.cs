namespace AssociationRegistry.Test.Projections.Tests.When_Saving_A_Document_To_Elastic;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using AutoFixture;
using Xunit;

public class Given_A_Valid_VerenigingDocument_Fixture : ElasticRepositoryFixture
{
    public Given_A_Valid_VerenigingDocument_Fixture() : base(nameof(Given_A_Valid_VerenigingDocument_Fixture))
    {
    }
}

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

        _classFixture.ElasticRepository
            .Index(
                new VerenigingDocument(
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<VerenigingDocument.Locatie>(),
                    new[] { fixture.Create<VerenigingDocument.Locatie>() },
                    new[] { fixture.Create<string>() },
                    fixture.Create<string>(),
                    new[] { fixture.Create<string>() }
                ));
    }
}
