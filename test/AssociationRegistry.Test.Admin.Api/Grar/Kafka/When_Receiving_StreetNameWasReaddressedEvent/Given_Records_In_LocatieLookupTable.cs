namespace AssociationRegistry.Test.Admin.Api.Grar.Kafka.When_Receiving_StreetNameWasReaddressedEvent;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using Xunit;
using FluentAssertions;

public class Given_LocatieLookup_Records
{
    public Given_LocatieLookup_Records()
    {
    }

    [Fact]
    public async Task Then_Messages_Are_Queued()
    {
        var locatieFinder = new LocatieFinder(new List<LocatieLookupDocument>()
        {
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "456",
                LocatieId = 1
            }
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder);

        var result = await sut.ForAddress("123", "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage("VCode1", new List<LocatieIdWithAdresId>() { new(1, "123") }, "idempotencyKey")
        });
    }
}

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode
{
    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode()
    {
    }

    [Fact]
    public async Task Then_Messages_Are_Queued()
    {
        var locatieFinder = new LocatieFinder(new List<LocatieLookupDocument>()
        {
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "456",
                LocatieId = 1
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 2
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1
            },
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder);

        var result = await sut.ForAddress("123", "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage("VCode1", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "123") }, "idempotencyKey"),
            new TeHeradresserenLocatiesMessage("VCode2", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "123"), new LocatieIdWithAdresId(2, "123") }, "idempotencyKey")
        });
    }
}
