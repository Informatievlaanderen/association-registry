﻿namespace AssociationRegistry.Test.Admin.Api.Grar.Kafka.When_Receiving_StreetNameWasReaddressedEvent;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Xunit;
using FluentAssertions;
using Framework;
using Moq;

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
        var locatieFinder = new FakeLocatieFinder(new List<LocatieLookupDocument>()
        {
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "456",
                LocatieId = 2,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "789",
                LocatieId = 2,
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
                                               "idempotencyKey"),
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
        var locatieFinder = new FakeLocatieFinder(new List<LocatieLookupDocument>()
        {
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode1",
                AdresId = "456",
                LocatieId = 2,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1,
            },
            new LocatieLookupDocument()
            {
                VCode = "VCode2",
                AdresId = "789",
                LocatieId = 2,
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
                                               "idempotencyKey"),
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
                                                                                readdressedHouseNumber: CreateReaddressedAddressData(
                                                                                    vanAdresId: 1000, naarAdresId: 2000),
                                                                                readdressedBoxNumbers: new List<ReaddressedAddressData>()
                                                                                {
                                                                                    CreateReaddressedAddressData(
                                                                                        vanAdresId: 1001, naarAdresId: 2001),
                                                                                    CreateReaddressedAddressData(
                                                                                        vanAdresId: 1002, naarAdresId: 2002),
                                                                                });

        var secondHouseNumberToReaddress = new AddressHouseNumberReaddressedData(addressPersistentLocalId: 2000, // can be ignored
                                                                                 readdressedHouseNumber: CreateReaddressedAddressData(
                                                                                     vanAdresId: 2000, naarAdresId: 3000),
                                                                                 readdressedBoxNumbers: new List<ReaddressedAddressData>()
                                                                                 {
                                                                                     CreateReaddressedAddressData(
                                                                                         vanAdresId: 2001, naarAdresId: 3002),
                                                                                     CreateReaddressedAddressData(
                                                                                         vanAdresId: 2002, naarAdresId: 3003),
                                                                                 });

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>()
        {
            firstHouseNumberToReaddress,
            secondHouseNumberToReaddress,
        };

        var vCode1 = "VCode1";
        var vCode2 = "VCode2";

        var locatieLookupHuisnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            1,
            firstHouseNumberToReaddress.ReaddressedHouseNumber.SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedHouseNumber.DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer1Busnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            1,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer1Busnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            2,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            2,
            firstHouseNumberToReaddress.ReaddressedHouseNumber.SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedHouseNumber.DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2Busnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            3,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2Busnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            3,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].DestinationAddressPersistentLocalId.ToString());

        var sut = new TeHeradresserenLocatiesMapper(new FakeLocatieFinder(new List<LocatieLookupDocument>()
        {
            locatieLookupHuisnummer1.Document, //vcode1
            locatieLookupHuisnummer1Busnummer1.Document,//vcode2
            locatieLookupHuisnummer1Busnummer2.Document,//vcode1
            locatieLookupHuisnummer2.Document,//vcode2
            locatieLookupHuisnummer2Busnummer1.Document, //vcode1
            locatieLookupHuisnummer2Busnummer2.Document, //vcode2
        }));

        var result = await sut.ForAddress(readdressedHouseNumbers: addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(expectation: new List<TeHeradresserenLocatiesMessage>()
        {
            new TeHeradresserenLocatiesMessage(
                VCode: vCode1,
                LocatiesMetAdres: new List<LocatieIdWithAdresId>()
                {
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer1.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer1.ExpectedAdresId),
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer1Busnummer2.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer1Busnummer2.ExpectedAdresId),
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer2Busnummer1.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer2Busnummer1.ExpectedAdresId),
                },
                idempotencyKey: "idempotencyKey"),
            new TeHeradresserenLocatiesMessage(
                VCode: vCode2,
                LocatiesMetAdres: new List<LocatieIdWithAdresId>()
                {
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer1Busnummer1.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer1Busnummer1.ExpectedAdresId),
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer2.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer2.ExpectedAdresId),
                    new LocatieIdWithAdresId(LocatieId: locatieLookupHuisnummer2Busnummer2.Document.LocatieId,
                                             AddressId: locatieLookupHuisnummer2Busnummer2.ExpectedAdresId),
                },
                idempotencyKey: "idempotencyKey"),
        });
    }

    private LocatieLookupMetExpectedAdres CreateLocatieLookupMetExpectedAdres(
        string vCode,
        int locatieId,
        string vanAdresId,
        string naarAdresId)
        => new(
            Document: new LocatieLookupDocument()
            {
                VCode = vCode,
                AdresId = vanAdresId,
                LocatieId = locatieId,
            },
            ExpectedAdresId: naarAdresId);

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
    {
        return new ReaddressedAddressData(vanAdresId, naarAdresId, false, string.Empty, string.Empty, string.Empty,
                                          string.Empty, string.Empty, string.Empty, string.Empty, false);
    }
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);

public class FakeLocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public FakeLocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
        => _locatieLookupDocuments.Where(x => adresIds.Contains(x.AdresId));
}
