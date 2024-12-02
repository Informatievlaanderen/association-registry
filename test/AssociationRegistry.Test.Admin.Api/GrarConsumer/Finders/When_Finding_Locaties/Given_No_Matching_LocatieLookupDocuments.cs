namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.Finders.When_Finding_Locaties;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Grar.LocatieFinder;
using Marten;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class LocatieLookupFixture : IAsyncLifetime
{
    public Fixture AutoFixture { get; private set; }
    public DocumentStore Store { get; private set; }
    public IDocumentSession Session { get; private set; }

    public async Task InitializeAsync()
    {
        AutoFixture = new Fixture().CustomizeAdminApi();

        Store = await TestDocumentStoreFactory.Create(nameof(LocatieLookupFixture));
        Session = Store.LightweightSession();
    }

    public async Task DisposeAsync()
    {
    }
}

[IntegrationTest]
public class Given_No_Matching_LocatieLookupDocuments : IClassFixture<LocatieLookupFixture>
{
    private readonly LocatieLookupFixture _fixture;

    public Given_No_Matching_LocatieLookupDocuments(LocatieLookupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Returns_Empty_Collection()
    {
        var locatieLookupDocument = _fixture.AutoFixture.Create<LocatieLookupDocument>();

        _fixture.Session.Store(locatieLookupDocument);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(_fixture.AutoFixture.Create<string>());

        actual.Should().BeEmpty();
    }
}

[IntegrationTest]
public class Given_A_Single_Matching_LocatieLookupDocuments : IClassFixture<LocatieLookupFixture>
{
    private readonly LocatieLookupFixture _fixture;

    public Given_A_Single_Matching_LocatieLookupDocuments(LocatieLookupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Returns_A_Collection()
    {
        var locatieLookupDocument = _fixture.AutoFixture.Create<LocatieLookupDocument>();

        _fixture.Session.Store(locatieLookupDocument);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(locatieLookupDocument.AdresId);

        actual.Should().BeEquivalentTo(LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, int[]>()
        {
            { locatieLookupDocument.VCode, [locatieLookupDocument.LocatieId] },
        }));
    }
}

[IntegrationTest]
public class Given_A_Multiple_Matching_LocatieLookupDocuments : IClassFixture<LocatieLookupFixture>
{
    private readonly LocatieLookupFixture _fixture;

    public Given_A_Multiple_Matching_LocatieLookupDocuments(LocatieLookupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Returns_A_Collection()
    {
        var vCode1 = _fixture.AutoFixture.Create<VCode>();
        var vCode2 = _fixture.AutoFixture.Create<VCode>();
        var adresId1 = _fixture.AutoFixture.Create<string>();

        LocatieLookupDocument[] locatieLookupDocument =
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

        _fixture.Session.Store(locatieLookupDocument);
        await _fixture.Session.SaveChangesAsync();

        var sut = new LocatieFinder(_fixture.Store);

        var actual = await sut.FindLocaties(adresId1);

        actual.Should().BeEquivalentTo(LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, int[]>()
        {
            { vCode1, [locatieLookupDocument[0].LocatieId, locatieLookupDocument[2].LocatieId] },
            { vCode2, [locatieLookupDocument[1].LocatieId] },
        }));
    }
}
