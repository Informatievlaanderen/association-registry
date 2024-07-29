namespace AssociationRegistry.Test.Admin.Api;

using AssociationRegistry.Admin.Api.VCodeGeneration;
using AssociationRegistry.Test.Admin.Api.Fakes;
using AssociationRegistry.Vereniging;
using Marten;
using Xunit;

public class VCodeServiceTests
{
    [Fact]
    public void It_Should_GetNext_From_Database()
    {
        var vCodeService = new SequenceVCodeService(new DocumentStore(new StoreOptions()));
        var vCode = vCodeService.GetNext();

    }

    [Fact]
    public void It_Should_GetNext_From_InMemory()
    {
        var vCodeService = new InMemorySequentialVCodeService();

        var vCode = vCodeService.GetNext();
    }

}
