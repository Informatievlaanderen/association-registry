namespace AssociationRegistry.Test.Admin.Api.Grar.Kafka.When_Receiving_StreetNameWasReaddressedEvent;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Be.Vlaanderen.Basisregisters.Utilities;
using Xunit;
using FluentAssertions;
using Framework;

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode
{
    private readonly Fixture _fixture;

    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode()
    {
        _fixture = new Fixture().CustomizeAdminApi();
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
                LocatieId = 2
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "789",
                LocatieId = 2
            },
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder);

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>()
        {
            new AddressHouseNumberReaddressedData(777, // can be ignored
                                                  CreateReaddressedAddressData(123, 777), // vanAdresId, naarAdresId (HouseNumber)
                                                  new List<ReaddressedAddressData>() // BoxNumbers
                                                  {
                                                  }),
        };

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage("VCode1", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "777") },
                                               "idempotencyKey"),
            new TeHeradresserenLocatiesMessage("VCode2", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "777") },
                                               "idempotencyKey")
        });
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
    {
        return new ReaddressedAddressData(vanAdresId, naarAdresId, false, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false);
    }
}

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode_Grouping
{
    private readonly Fixture _fixture;

    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode_Grouping()
    {
        _fixture = new Fixture().CustomizeAdminApi();
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
                LocatieId = 2
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "789",
                LocatieId = 2
            },
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder);

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>()
        {
            new AddressHouseNumberReaddressedData(777, // can be ignored
                                                  CreateReaddressedAddressData(123, 777), // vanAdresId, naarAdresId (HouseNumber)
                                                  new List<ReaddressedAddressData>() // BoxNumbers
                                                  {
                                                  }),
            new AddressHouseNumberReaddressedData(777, // can be ignored
                                                  CreateReaddressedAddressData(456, 888), // vanAdresId, naarAdresId (HouseNumber)
                                                  new List<ReaddressedAddressData>() // BoxNumbers
                                                  {
                                                  }),
        };

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage(
                "VCode1", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "777"), new LocatieIdWithAdresId(2, "888") },
                "idempotencyKey"),
            new TeHeradresserenLocatiesMessage("VCode2", new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(1, "777") },
                                               "idempotencyKey")
        });
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
    {
        return new ReaddressedAddressData(vanAdresId, naarAdresId, false, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false);
    }
}

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode_With_BoxNumbers
{
    private readonly Fixture _fixture;

    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode_With_BoxNumbers()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task Then_Messages_Are_Queued()
    {
        var firstHouseNumberToReaddress = new AddressHouseNumberReaddressedData(addressPersistentLocalId: 1000, // can be ignored
                                                                               readdressedHouseNumber: CreateReaddressedAddressData(vanAdresId: 1000, naarAdresId: 2000),
                                                                               readdressedBoxNumbers: new List<ReaddressedAddressData>()
                                                                               {
                                                                                   CreateReaddressedAddressData(vanAdresId: 1001, naarAdresId: 2001),
                                                                                   CreateReaddressedAddressData(vanAdresId: 1002, naarAdresId: 2002)
                                                                               });

        var secondHouseNumberToReaddress = new AddressHouseNumberReaddressedData(addressPersistentLocalId: 2000, // can be ignored
                                                                               readdressedHouseNumber: CreateReaddressedAddressData(vanAdresId: 2000, naarAdresId: 3000),
                                                                               readdressedBoxNumbers: new List<ReaddressedAddressData>()
                                                                               {
                                                                                   CreateReaddressedAddressData(vanAdresId: 2001, naarAdresId: 3002),
                                                                                   CreateReaddressedAddressData(vanAdresId: 2002, naarAdresId: 3003)
                                                                               });

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>()
        {
            firstHouseNumberToReaddress,
            secondHouseNumberToReaddress,
        };

        var locatieLookupMetExpectedAdres1 = new LocatieLookupMetExpectedAdres(
            Document: new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = firstHouseNumberToReaddress.ReaddressedHouseNumber.SourceAddressPersistentLocalId.ToString(),
                LocatieId = 1
            },
            ExpectedAdresId: firstHouseNumberToReaddress.ReaddressedHouseNumber.DestinationHouseNumber);

        var locatieLookupMetExpectedAdres2 = new LocatieLookupMetExpectedAdres(
            Document: new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1
            },
            ExpectedAdresId: "123");


        var locatieFinder = new LocatieFinder(locatieLookupDocuments: new List<LocatieLookupDocument>()
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
                LocatieId = 2
            }
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder: locatieFinder);



        var result = await sut.ForAddress(readdressedHouseNumbers: addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(expectation: new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage(
                VCode: "VCode1", LocatiesMetAdres: new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(LocatieId: 1, AddressId: "666"), new LocatieIdWithAdresId(LocatieId: 2, AddressId: "888") },
                idempotencyKey: "idempotencyKey"),
            new TeHeradresserenLocatiesMessage(VCode: "VCode2", LocatiesMetAdres: new List<LocatieIdWithAdresId>() { new LocatieIdWithAdresId(LocatieId: 1, AddressId: "777") },
                                               idempotencyKey: "idempotencyKey")
        });
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
    {
        return new ReaddressedAddressData(vanAdresId, naarAdresId, false, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false);
    }
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);
