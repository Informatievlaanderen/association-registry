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