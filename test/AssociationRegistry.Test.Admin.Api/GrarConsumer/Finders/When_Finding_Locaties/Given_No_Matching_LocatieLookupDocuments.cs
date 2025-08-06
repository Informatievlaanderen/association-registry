namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.Finders.When_Finding_Locaties;

using AssociationRegistry.Admin.Api.HostedServices.GrarKafkaConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Marten;
using Vereniging;
using Xunit;

public class LocatieLookupFixture : IAsyncLifetime
{
    public Fixture AutoFixture { get; private set; }
    public DocumentStore Store { get; private set; }
    public IDocumentSession Session { get; private set; }

    public async ValueTask InitializeAsync()
    {
        AutoFixture = new Fixture().CustomizeAdminApi();

        Store = await TestDocumentStoreFactory.CreateAsync(nameof(LocatieLookupFixture));
        Session = Store.LightweightSession();
    }

    public async ValueTask DisposeAsync()
    {
    }
}

public class Given_LocatieLookupDocuments : IClassFixture<LocatieLookupFixture>
{
    private readonly LocatieLookupFixture _fixture;

    public Given_LocatieLookupDocuments(LocatieLookupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask With_No_Matching_Documents_Then_Returns_Empty_Collection()
    {
        var locatieLookupDocument = _fixture.AutoFixture.Create<LocatieLookupDocument>();

        _fixture.Session.Store(locatieLookupDocument);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(_fixture.AutoFixture.Create<string>());

        actual.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask With_One_Matching_Documents_Then_Returns_A_Collection_With_One_Match()
    {
        var locatieLookupDocument = _fixture.AutoFixture.Create<LocatieLookupDocument>();

        _fixture.Session.Store(locatieLookupDocument);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(locatieLookupDocument.AdresId);

        actual.Should().BeEquivalentTo(LocatiesPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, LocatieLookupData[]>()
        {
            { locatieLookupDocument.VCode,
                [new LocatieLookupData(locatieLookupDocument.LocatieId, locatieLookupDocument.AdresId, locatieLookupDocument.VCode)]
            },
        }));
    }

    [Fact]
    public async ValueTask With_Multiple_Matching_Documents_Then_Returns_A_Collection_With_Multiple_Matches()
    {
        var vCode1 = _fixture.AutoFixture.Create<VCode>();
        var vCode2 = _fixture.AutoFixture.Create<VCode>();
        var adresId1 = _fixture.AutoFixture.Create<string>();

        LocatieLookupDocument[] locatieLookupData =
        [
            _fixture.AutoFixture.Create<LocatieLookupDocument>() with
            {
                VCode = vCode1,
                AdresId = adresId1
            },
            _fixture.AutoFixture.Create<LocatieLookupDocument>() with
            {
                VCode = vCode2,
                AdresId = adresId1
            },
            _fixture.AutoFixture.Create<LocatieLookupDocument>() with
            {
                VCode = vCode1,
                AdresId = adresId1
            },
        ];

        _fixture.Session.Store(locatieLookupData);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(adresId1);

        actual.Should().BeEquivalentTo(LocatiesPerVCodeCollection.FromLocatiesPerVCode(new()
        {
            {
                vCode1,
                [
                    new LocatieLookupData(locatieLookupData[0].LocatieId, locatieLookupData[0].AdresId, locatieLookupData[0].VCode),
                    new LocatieLookupData(locatieLookupData[2].LocatieId, locatieLookupData[2].AdresId, locatieLookupData[2].VCode),
                ]
            },
            {
                vCode2,
                [
                    new LocatieLookupData(locatieLookupData[1].LocatieId, locatieLookupData[1].AdresId, locatieLookupData[1].VCode),
                ]
            },
        }));
    }
}
