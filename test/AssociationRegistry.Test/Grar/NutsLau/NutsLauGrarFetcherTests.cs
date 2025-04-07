namespace AssociationRegistry.Test.Grar.NutsLau;

using AssociationRegistry.Grar.Clients;
using Moq;
using Xunit;

public class NutsLauGrarFetcherTests
{
    [Fact]
    public void XXX()
    {
        var client = new Mock<IGrarClient>();
        var sut = new NutsLauGrarFetcher(client.Object);


    }
}

public class NutsLauGrarFetcher
{
    private readonly IGrarClient _client;

    public NutsLauGrarFetcher(IGrarClient client)
    {
        _client = client;
    }

}
