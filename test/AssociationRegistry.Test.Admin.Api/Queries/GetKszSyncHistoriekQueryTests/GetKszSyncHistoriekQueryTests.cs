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
    public BeheerKszSyncHistoriekGebeurtenisDocument[] BeheerKszSyncHistoriekGebeurtenisAllDocuments;
    public BeheerKszSyncHistoriekGebeurtenisDocument DocumentWithSameVCode;

    public async ValueTask InitializeAsync()
    {
        _fixture = new Fixture().CustomizeDomain();

        Store = await TestDocumentStoreFactory.CreateAsync(nameof(KszSyncHistoriekQueryTestsTests));
        var session = Store.LightweightSession();

        BeheerKszSyncHistoriekGebeurtenisAllDocuments = _fixture.CreateMany<BeheerKszSyncHistoriekGebeurtenisDocument>().ToArray();
        DocumentWithSameVCode = _fixture.Create<BeheerKszSyncHistoriekGebeurtenisDocument>() with
        {
            VCode = BeheerKszSyncHistoriekGebeurtenisAllDocuments.First().VCode,
        };
        BeheerKszSyncHistoriekGebeurtenisAllDocuments = BeheerKszSyncHistoriekGebeurtenisAllDocuments.Append(DocumentWithSameVCode).ToArray();

        session.InsertObjects(BeheerKszSyncHistoriekGebeurtenisAllDocuments);

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
    private readonly BeheerKszSyncHistoriekGebeurtenisDocument _documentWithSameVCode;

    public KszSyncHistoriekQueryTestsTests(KszSyncHistoriekQueryTestsFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
        _fixture = setupFixture._fixture;
        _insertedDocs = setupFixture.BeheerKszSyncHistoriekGebeurtenisAllDocuments;
        _documentWithSameVCode = setupFixture.DocumentWithSameVCode;
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
    public async ValueTask With_Multiple_Matching_VCode_Then_Return_Multiple_Matching_Docs()
    {
        var query = new KszSyncHistoriekQuery(_session);

        var expectedVCode = _insertedDocs.First();

        var actual = await query.ExecuteAsync(
            new KszSyncHistoriekFilter(expectedVCode.VCode),
            CancellationToken.None
        );

        actual.Should().BeEquivalentTo([expectedVCode, _documentWithSameVCode]);
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
