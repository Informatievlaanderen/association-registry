namespace AssociationRegistry.Test.Admin.Api.Magda.MagdaKboService.When_Retrieving_VerenigingVolgensKbo;

using AssociationRegistry.Admin.Api.Magda;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AutoFixture;
using FluentAssertions;
using Framework;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaFacade>();
        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
            .ReturnsAsync(_fixture.Create<Envelope<GeefOndernemingResponseBody>>());

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object, new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(),_fixture.Create<string>(), CancellationToken.None);
        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async Task Then_It_Returns_A_VerenigingVolgensKbo()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(),_fixture.Create<string>(), CancellationToken.None);
        result.Should().BeOfType<Result<VerenigingVolgensKbo>>();
        //verenigignVolgensKboResult.Data.Should().BeEquivalentTo(new VerenigingVolgensKbo());
    }
}
