namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Moq;

public class When_Loading_With_Dubbels_TestBase
{
    protected Fixture _fixture;

    public When_Loading_With_Dubbels_TestBase()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    protected static async Task VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(Func<IVerenigingsRepository, Task> handleFunc)
    {
        var repositoryMock = new Mock<IVerenigingsRepository>();

        try
        {
            await handleFunc(repositoryMock.Object);
        }
        catch
        {
        }
        finally
        {
            repositoryMock.Verify(x => x.Load<VerenigingOfAnyKind>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), false, true), times: Times.Once);
        }
    }
}
