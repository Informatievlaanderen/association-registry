namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using FluentAssertions;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Xunit;

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode
{
    private readonly Fixture _fixture;

    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task Then_Messages_Are_GroupedBy_VCode()
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
        };

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(new List<TeHeradresserenLocatiesMessage>
        {
            new(VCode: "VCode1", new List<TeHeradresserenLocatie>
                    { new(LocatieId: 1, DestinationAdresId: "777") },
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
