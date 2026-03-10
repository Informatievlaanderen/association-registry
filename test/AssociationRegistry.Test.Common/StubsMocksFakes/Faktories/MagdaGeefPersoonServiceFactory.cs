namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using global::AutoFixture;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using Magda.Persoon;
using Moq;

public class MagdaGeefPersoonServiceFactory
{
    private readonly IFixture _fixture;
    private readonly Mock<IMagdaGeefPersoonService> _mock;

    public MagdaGeefPersoonServiceFactory(IFixture fixture)
    {
        _fixture = fixture;
        _mock = new Mock<IMagdaGeefPersoonService>();
    }

    public Mock<IMagdaGeefPersoonService> ReturnsNietOverledenPersoon(string? inszToReturn = null)
    {
        _mock
            .Setup(x =>
                x.GeefPersoon(
                    It.IsAny<GeefPersoonRequest>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PersoonUitKsz(inszToReturn ?? _fixture.Create<Insz>(), false));

        return _mock;
    }

    public Mock<IMagdaGeefPersoonService> ReturnsOverledenPersoon(string? inszToReturn = null)
    {
        _mock
            .Setup(x =>
                x.GeefPersoon(
                    It.IsAny<GeefPersoonRequest>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PersoonUitKsz(inszToReturn ?? _fixture.Create<Insz>(), true));

        return _mock;
    }
}
