namespace AssociationRegistry.Test.When_Address_Match.MessageHandling;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressMatch;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Removed_Vereniging
{
    [Fact]
    public async Task Then_Nothing()
    {

        var verenigingRepositoryMock = new Mock<IVerenigingsRepository>();

        var vCode = VCode.Create(1001001);

        verenigingRepositoryMock.Setup(s => s.Load<VerenigingOfAnyKind>(It.IsAny<VCode>(),
                                                                        It.IsAny<long?>()))
                                .ThrowsAsync(new VerenigingWerdVerwijderd(vCode));

        var grarClient = new Mock<IGrarClient>();

        var adresMatchenLocatieMessageHandler = new TeAdresMatchenLocatieMessageHandler(verenigingRepositoryMock.Object,
                                                                                        grarClient.Object,
                                                                                        new NullLogger<
                                                                                            TeAdresMatchenLocatieMessageHandler>()
        );

        await adresMatchenLocatieMessageHandler.Handle(new TeAdresMatchenLocatieMessage(vCode, 1));
    }
}
