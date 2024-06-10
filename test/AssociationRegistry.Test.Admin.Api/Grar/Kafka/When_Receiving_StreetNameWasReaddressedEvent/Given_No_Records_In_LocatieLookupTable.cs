namespace AssociationRegistry.Test.Admin.Api.Grar.Kafka.When_Receiving_StreetNameWasReaddressedEvent;

using Xunit;
using FluentAssertions;

public class Given_No_Records_In_LocatieLookupTable
{
    public Given_No_Records_In_LocatieLookupTable()
    {

    }

    [Fact]
    public async Task Then_No_Messages_Are_Queued()
    {
        var sut = new LocatieFinder();
        var result = await sut.ForAddress("1");

        result.Should().BeEmpty();
    }
}

public class LocatieFinder
{
    public async Task<IEnumerable<object>> ForAddress(string AdresId)
    {
        return  ArraySegment<object>.Empty;
    }
}
