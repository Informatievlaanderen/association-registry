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

        var addressHouseNumberReaddressedData = new List<AddressHouseNumberReaddressedData>
        {
            firstHouseNumberToReaddress,
            secondHouseNumberToReaddress,
        };

        var vCode1 = "VCode1";
        var vCode2 = "VCode2";

        var locatieLookupHuisnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            locatieId: 1,
            firstHouseNumberToReaddress.ReaddressedHouseNumber.SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedHouseNumber.DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer1Busnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            locatieId: 1,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer1Busnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            locatieId: 2,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            locatieId: 2,
            firstHouseNumberToReaddress.ReaddressedHouseNumber.SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedHouseNumber.DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2Busnummer1 = CreateLocatieLookupMetExpectedAdres(
            vCode1,
            locatieId: 3,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[0].DestinationAddressPersistentLocalId.ToString());

        var locatieLookupHuisnummer2Busnummer2 = CreateLocatieLookupMetExpectedAdres(
            vCode2,
            locatieId: 3,
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].SourceAddressPersistentLocalId.ToString(),
            firstHouseNumberToReaddress.ReaddressedBoxNumbers[1].DestinationAddressPersistentLocalId.ToString());

        var sut = new TeHeradresserenLocatiesMapper(new FakeLocatieFinder(new List<LocatieLookupDocument>
        {
            locatieLookupHuisnummer1.Document, //vcode1
            locatieLookupHuisnummer1Busnummer1.Document, //vcode2
            locatieLookupHuisnummer1Busnummer2.Document, //vcode1
            locatieLookupHuisnummer2.Document, //vcode2
            locatieLookupHuisnummer2Busnummer1.Document, //vcode1
            locatieLookupHuisnummer2Busnummer2.Document, //vcode2
        }));

        var result = await sut.ForAddress(addressHouseNumberReaddressedData, idempotenceKey: "idempotencyKey");

        result.Should().BeEquivalentTo(expectation: new List<TeHeradresserenLocatiesMessage>
        {
            new(
                vCode1,
                new List<TeHeradresserenLocatie>
                {
                    new(locatieLookupHuisnummer1.Document.LocatieId,
                        locatieLookupHuisnummer1.ExpectedAdresId),
                    new(locatieLookupHuisnummer1Busnummer2.Document.LocatieId,
                        locatieLookupHuisnummer1Busnummer2.ExpectedAdresId),
                    new(locatieLookupHuisnummer2Busnummer1.Document.LocatieId,
                        locatieLookupHuisnummer2Busnummer1.ExpectedAdresId),
                },
                idempotencyKey: "idempotencyKey"),
            new(
                vCode2,
                new List<TeHeradresserenLocatie>
                {
                    new(locatieLookupHuisnummer1Busnummer1.Document.LocatieId,
                        locatieLookupHuisnummer1Busnummer1.ExpectedAdresId),
                    new(locatieLookupHuisnummer2.Document.LocatieId,
                        locatieLookupHuisnummer2.ExpectedAdresId),
                    new(locatieLookupHuisnummer2Busnummer2.Document.LocatieId,
                        locatieLookupHuisnummer2Busnummer2.ExpectedAdresId),
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
            new LocatieLookupDocument
            {
                VCode = vCode,
                AdresId = vanAdresId,
                LocatieId = locatieId,
            },
            naarAdresId);

    public ReaddressedAddressData CreateReaddressedAddressData(int vanAdresId, int naarAdresId)
        => new(vanAdresId, naarAdresId, isDestinationNewlyProposed: false, string.Empty, string.Empty, string.Empty,
               string.Empty, string.Empty, string.Empty, string.Empty, sourceIsOfficiallyAssigned: false);
}
