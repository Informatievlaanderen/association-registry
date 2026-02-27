namespace AssociationRegistry.Test.Admin.Api.Queries.GetKszSyncHistoriekQueryTests;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;

public class KszSyncHistoriekQueryTestsFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }
    public Fixture _fixture;
    public BeheerKszSyncHistoriekGebeurtenisDocument[] BeheerKszSyncHistoriekGebeurtenisDocuments;

    public async ValueTask InitializeAsync()
    {
        _fixture = new Fixture().CustomizeDomain();

        Store = await TestDocumentStoreFactory.CreateAsync(nameof(KszSyncHistoriekQueryTestsTests));
        var session = Store.LightweightSession();

        BeheerKszSyncHistoriekGebeurtenisDocuments = _fixture.CreateMany<BeheerKszSyncHistoriekGebeurtenisDocument>().ToArray();
        session.InsertObjects(BeheerKszSyncHistoriekGebeurtenisDocuments);

        await session.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class KszSyncHistoriekQueryTestsTests
    : IClassFixture<KszSyncHistoriekQueryTestsFixture>,
        IDisposable,
        IAsyncDisposable
{
    private readonly IDocumentSession _session;
    private readonly Fixture _fixture;
    private readonly BeheerKszSyncHistoriekGebeurtenisDocument[] _insertedDocs;

    public KszSyncHistoriekQueryTestsTests(KszSyncHistoriekQueryTestsFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
        _fixture = setupFixture._fixture;
        _insertedDocs = setupFixture.BeheerKszSyncHistoriekGebeurtenisDocuments;
    }

    [Fact]
    public async ValueTask With_No_VCode_Then_Return_All_Docs()
    {
        var query = new KszSyncHistoriekQuery(_session);

        var actual = await query.ExecuteAsync(
            new KszSyncHistoriekFilter(),
            CancellationToken.None
        );

        actual.Should().BeEquivalentTo(_insertedDocs);
    }

    [Fact]
    public async ValueTask With_No_Matching_VCode_Then_Return_No_Docs()
    {
        var query = new KszSyncHistoriekQuery(_session);

        var actual = await query.ExecuteAsync(
            new KszSyncHistoriekFilter(_fixture.Create<VCode>()),
            CancellationToken.None
        );

        actual.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask With_Matching_VCode_Then_Return_Matching_Docs()
    {
        var query = new KszSyncHistoriekQuery(_session);

        var expectedDoc = _insertedDocs.First();

        var actual = await query.ExecuteAsync(
            new KszSyncHistoriekFilter(expectedDoc.VCode),
            CancellationToken.None
        );

        actual.Should().BeEquivalentTo([expectedDoc]);
    }

    [Fact]
    public async ValueTask With_Multiple_Matching_VCode_Then_Return_Multiple_Matching_Docs()
    {
        var query = new KszSyncHistoriekQuery(_session);

        var expectedDoc = _insertedDocs.First();

        var extraDocWithSameVCode = await InsertBeheerKszSyncHistoriekGebeurtenisDocument(expectedDoc);

        var actual = await query.ExecuteAsync(
            new KszSyncHistoriekFilter(expectedDoc.VCode),
            CancellationToken.None
        );

        actual.Should().BeEquivalentTo([expectedDoc, extraDocWithSameVCode]);
    }

    private async ValueTask<BeheerKszSyncHistoriekGebeurtenisDocument> InsertBeheerKszSyncHistoriekGebeurtenisDocument(BeheerKszSyncHistoriekGebeurtenisDocument expectedDoc)
    {
        var extraDocWithSameVCode = _fixture.Create<BeheerKszSyncHistoriekGebeurtenisDocument>() with
        {
            VCode = expectedDoc.VCode,
        };

        _session.Insert(extraDocWithSameVCode);
        await _session.SaveChangesAsync();

        return extraDocWithSameVCode;
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync();
    }
}
