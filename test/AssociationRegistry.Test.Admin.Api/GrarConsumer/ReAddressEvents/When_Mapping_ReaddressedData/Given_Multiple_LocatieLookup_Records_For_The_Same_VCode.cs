namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.ReAddressEvents.When_Mapping_ReaddressedData;

using AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Common.AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_Multiple_LocatieLookup_Records_For_The_Same_VCode
{
    private readonly Fixture _fixture;

    public Given_Multiple_LocatieLookup_Records_For_The_Same_VCode()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask Then_Messages_Are_GroupedBy_VCode()
    {

        var locatieFinderMock = new Mock<ILocatieFinder>();

        locatieFinderMock.Setup(x => x.FindLocaties(It.IsAny<string[]>()))
                         .ReturnsAsync(LocatiesPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, LocatieLookupData[]>()
                          {
                              { "VCode1", [new LocatieLookupData(1, "123", "VCode1"), new LocatieLookupData(2, "456", "VCode1")] },
                              { "VCode2", [new LocatieLookupData(1, "123", "VCode2"), new LocatieLookupData(2, "789", "VCode2")] },
                          }));

        var sut = new TeHeradresserenLocatiesMapper(locatieFinderMock.Object);

        var addressHouseNumberReaddressedData = new TeHernummerenStraat(new List<TeHernummerenAdres>
        {
            new(VanAdresId: 123, NaarAdresId: 777),
            new(VanAdresId: 456, NaarAdresId: 888),
            new(VanAdresId: 789, NaarAdresId: 999),
        });

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(new List<HeradresseerLocatiesMessage>
        {
            new(VCode: "VCode1", new List<TeHeradresserenLocatie>
                {
                    new(LocatieId: 1, NaarAdresId: "777"),
                    new(LocatieId: 2, NaarAdresId: "888"),
                },
                idempotencyKey: "idempotencyKey"),
            new(VCode: "VCode2", new List<TeHeradresserenLocatie>
                {
                    new(LocatieId: 1, NaarAdresId: "777"),
                    new(LocatieId: 2, NaarAdresId: "999"),
                },
                idempotencyKey: "idempotencyKey"),
        });
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
        => new(vanAdresId, naarAdresId, isDestinationNewlyProposed: false, string.Empty, string.Empty, string.Empty,
               string.Empty, string.Empty, string.Empty, string.Empty, sourceIsOfficiallyAssigned: false);
}
