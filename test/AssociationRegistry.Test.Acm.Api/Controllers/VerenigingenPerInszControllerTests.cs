namespace AssociationRegistry.Test.Acm.Api.Controllers;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using FluentAssertions;
using Moq;
using Xunit;

public class VerenigingenPerInszControllerTests
{
    [Fact]
    public async Task VerifyQueryIsCalled()
    {
        var cancellationToken = CancellationToken.None;
        var mockQuery = new Mock<IVerenigingenPerInszQuery>();
        var insz = "123";
        var verenigingenPerInszRequest = new VerenigingenPerInszRequest(){Insz = insz};

        mockQuery.Setup(x => x.ExecuteAsync(
                            It.Is<VerenigingenPerInszFilter>(filter => filter.Insz == insz),
                            It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new VerenigingenPerInszDocument());

        var sut = new VerenigingenPerInszController();

        var result = await sut.Get(mockQuery.Object, verenigingenPerInszRequest, CancellationToken.None);

        result.Should().BeEquivalentTo(new VerenigingenPerInszResponse() { Insz = insz });
    }
}

