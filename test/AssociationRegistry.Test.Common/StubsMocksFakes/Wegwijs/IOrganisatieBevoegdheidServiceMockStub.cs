namespace AssociationRegistry.Test.Common.StubsMocksFakes.Wegwijs;

using AssociationRegistry.Wegwijs;
using Moq;

public class IOrganisatieBevoegdheidServiceMockStub
{
    private Mock<IOrganisatieBevoegdheidService> _mock = new Mock<IOrganisatieBevoegdheidService>();

    public IOrganisatieBevoegdheidService Object => _mock.Object;

    public IOrganisatieBevoegdheidServiceMockStub WithGemachtigdeOrganisaties(params string[] gemachtigdeOrganisaties)
    {
        _mock
            .Setup(x => x.GetOpvolgers(It.IsAny<string>()))
            .ReturnsAsync(gemachtigdeOrganisaties);

        return this;
    }

    public void VerifyOnce(string? initiator = null) =>
        _mock.Verify(
            x =>
                x.GetOpvolgers(
                    initiator ?? It.IsAny<string>()
                ),
            Times.Once
        );

    public void VerifyNever() =>
        _mock.Verify(x => x.GetOpvolgers(It.IsAny<string>()), Times.Never);
}
