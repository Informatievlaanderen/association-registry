namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarConsumer;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Common.AutoFixture;
using FluentAssertions;
using Framework;
using Grar.GrarConsumer.TeHeradresserenLocaties;
using Grar.Models;
using Xunit;

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
        var locatieFinder = new FakeLocatieFinder(new List<LocatieLookupDocument>
        {
            new()
            {
                VCode = "VCode1",
                AdresId = "123",
                LocatieId = 1,
            },
            new()
            {
                VCode = "VCode1",
                AdresId = "456",
                LocatieId = 2,
            },
            new()
            {
                VCode = "VCode2",
                AdresId = "123",
                LocatieId = 1,
            },
            new()
            {
                VCode = "VCode2",
                AdresId = "789",
                LocatieId = 2,
            },
        });

        var sut = new TeHeradresserenLocatiesMapper(locatieFinder);

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>
        {
            new(addressPersistentLocalId: 777, // can be ignored
                CreateReaddressedAddressData(vanAdresId: 123, naarAdresId: 777), // vanAdresId, naarAdresId (HouseNumber)
                new List<ReaddressedAddressData>() // BoxNumbers
            ),
            new(addressPersistentLocalId: 777, // can be ignored
                CreateReaddressedAddressData(vanAdresId: 456, naarAdresId: 888), // vanAdresId, naarAdresId (HouseNumber)
                new List<ReaddressedAddressData>() // BoxNumbers
            ),
        };

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>
        {
            new(
                VCode: "VCode1", new List<TeHeradresserenLocatie>
                    { new(LocatieId: 1, DestinationAdresId: "777"), new(LocatieId: 2, DestinationAdresId: "888") },
                idempotencyKey: "idempotencyKey"),
            new(VCode: "VCode2", new List<TeHeradresserenLocatie>
                    { new(LocatieId: 1, DestinationAdresId: "777") },
                idempotencyKey: "idempotencyKey"),
        });
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
        => new(vanAdresId, naarAdresId, isDestinationNewlyProposed: false, string.Empty, string.Empty, string.Empty,
               string.Empty, string.Empty, string.Empty, string.Empty, sourceIsOfficiallyAssigned: false);
}
