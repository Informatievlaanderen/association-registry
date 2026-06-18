namespace AssociationRegistry.Test.Common.StubsMocksFakes.Wegwijs;

using AssociationRegistry.Wegwijs;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Integrations.Wegwijs.Clients;
using Integrations.Wegwijs.Responses;
using Moq;

public class IWegwijsClientMockStub
{
    private Mock<IWegwijsClient> _mock = new Mock<IWegwijsClient>();

    public IWegwijsClient Object => _mock.Object;

    public IWegwijsClientMockStub SetupGemachtigdeOrganisatie(string ovoCode, string opvolger)
    {
        _mock
            .Setup(x => x.GetOrganisationByOvoCode(ovoCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(OrganisationWithOpvolger(opvolger));

        return this;
    }

    public IWegwijsClientMockStub SetupGemachtigdeOrganisatieWithoutOpvolgers(string ovoCode)
    {
        _mock
            .Setup(x => x.GetOrganisationByOvoCode(ovoCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrganisationResponse());

        return this;
    }

    public IWegwijsClientMockStub SetupThrowOrganisatieNietGevondenException(string ovoCode)
    {
        _mock
            .Setup(x => x.GetOrganisationByOvoCode(ovoCode, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OrganisatieNietGevondenException(string.Empty));

        return this;
    }

    private static OrganisationResponse OrganisationWithOpvolger(string opvolgerOvoCode) =>
        new()
        {
            Relations =
            [
                new OrganisationRelation
                {
                    RelationId = new Guid("2c68b8eb-55d2-ff8e-3301-f0fb12467df7"),
                    RelatedOrganisationOvoNumber = opvolgerOvoCode,
                },
            ],
        };

    // public void VerifyOnce(string? initiator = null) =>
    //     _mock.Verify(
    //         x =>
    //             x.GetOpvolgers(
    //                 initiator ?? It.IsAny<string>()
    //             ),
    //         Times.Once
    //     );
    //
    // public void VerifyNever() =>
    //     _mock.Verify(x => x.GetOpvolgers(It.IsAny<string>()), Times.Never);
}
