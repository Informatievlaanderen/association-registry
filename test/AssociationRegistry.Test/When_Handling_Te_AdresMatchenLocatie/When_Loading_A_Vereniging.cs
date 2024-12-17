namespace AssociationRegistry.Test.When_Handling_Te_AdresMatchenLocatie;

using AutoFixture;
using Common.AutoFixture;
using Grar;
using Grar.AddressMatch;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Serilog;
using Vereniging;
using Xunit;

public class When_Loading_A_Vereniging
{
    [Fact]
    public async Task Then_It_Should_Have_Loaded_AllowDubbels()
    {
        var fixture = new Fixture().CustomizeDomain();
        var repositoryMock = new Mock<IVerenigingsRepository>();
        var sut = new TeAdresMatchenLocatieMessageHandler(repositoryMock.Object, Mock.Of<IGrarClient>(), NullLogger<TeAdresMatchenLocatieMessageHandler>.Instance);

        try
        {
            await sut.Handle(fixture.Create<TeAdresMatchenLocatieMessage>() with
            {
                VCode = fixture.Create<VCode>(),
            }, CancellationToken.None);
        }
        catch
        {
        }
        finally
        {
            repositoryMock.Verify(x => x.Load<VerenigingOfAnyKind>(It.IsAny<VCode>(), It.IsAny<long?>(), false, true), times: Times.Once);
        }
    }
}
