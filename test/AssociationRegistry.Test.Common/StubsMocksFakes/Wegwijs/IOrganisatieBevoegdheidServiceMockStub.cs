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
            .Setup(x => x.GetAndValidateGemachtigdeOrganisaties(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gemachtigdeOrganisaties);

        return this;
    }

    public void VerifyOnce(string? initiator = null, string? geregistreerdDoor = null) =>
        _mock.Verify(
            x =>
                x.GetAndValidateGemachtigdeOrganisaties(
                    initiator ?? It.IsAny<string>(),
                    geregistreerdDoor ?? It.IsAny<string>()
                ),
            Times.Once
        );

    public void VerifyNever() =>
        _mock.Verify(x => x.GetAndValidateGemachtigdeOrganisaties(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
}
