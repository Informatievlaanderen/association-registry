namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.ReAddressEvents.When_Mapping_ReaddressData_To_TeHernummerenStraat;

using AssociationRegistry.Admin.Api.HostedServices.GrarKafkaConsumer.Kafka.StraatHernummering;
using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common;
using FluentAssertions;
using Xunit;

public class Given_ReaddressedData
{
    private readonly Fixture _fixture;

    public Given_ReaddressedData()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public void Then_It_Maps()
    {
                var firstHouseNumberToReaddress = new AddressHouseNumberReaddressedData(addressPersistentLocalId: 1000, // can be ignored
                                                                                CreateReaddressedAddressData(
                                                                                    vanAdresId: 1000, naarAdresId: 2000),
                                                                                new List<ReaddressedAddressData>
                                                                                {
                                                                                    CreateReaddressedAddressData(
                                                                                        vanAdresId: 1001, naarAdresId: 2001),
                                                                                    CreateReaddressedAddressData(
                                                                                        vanAdresId: 1002, naarAdresId: 2002),
                                                                                });

        var secondHouseNumberToReaddress = new AddressHouseNumberReaddressedData(addressPersistentLocalId: 2000, // can be ignored
                                                                                 CreateReaddressedAddressData(
                                                                                     vanAdresId: 2000, naarAdresId: 3000),
                                                                                 new List<ReaddressedAddressData>
                                                                                 {
                                                                                     CreateReaddressedAddressData(
                                                                                         vanAdresId: 2001, naarAdresId: 3002),
                                                                                     CreateReaddressedAddressData(
                                                                                         vanAdresId: 2002, naarAdresId: 3003),
                                                                                 });

        var actual = TeHernummerenStraatFactory.From(
            new StreetNameWasReaddressed(
                _fixture.Create<int>(),
                [firstHouseNumberToReaddress, secondHouseNumberToReaddress],
                _fixture.Create<Provenance>()));

        var expected = new TeHernummerenStraat(new List<TeHernummerenAdres>
        {
            new(VanAdresId: 1000, NaarAdresId: 2000),
            new(VanAdresId: 1001, NaarAdresId: 2001),
            new(VanAdresId: 1002, NaarAdresId: 2002),
            new(VanAdresId: 2000, NaarAdresId: 3000),
            new(VanAdresId: 2001, NaarAdresId: 3002),
            new(VanAdresId: 2002, NaarAdresId: 3003),
        });

        actual.Should().BeEquivalentTo(expected);
    }

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
        => new(vanAdresId, naarAdresId, isDestinationNewlyProposed: false, string.Empty, string.Empty, string.Empty,
               string.Empty, string.Empty, string.Empty, string.Empty, sourceIsOfficiallyAssigned: false);
}
