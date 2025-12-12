namespace AssociationRegistry.Test.Admin.Api.Queries.VertegenwoordigersPerVCodeQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.Admin.Schema.Vertegenwoordiger;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using BewaartermijnQuery;
using FluentAssertions;
using Marten;
using Xunit;

[CollectionDefinition(nameof(NonParallelDbCollection), DisableParallelization = true)]
public class NonParallelDbCollection { }

public class VertegenwoordigersPerVCodeQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(VertegenwoordigersPerVCodeQueryTests));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[Collection(nameof(NonParallelDbCollection))]
public class VertegenwoordigersPerVCodeQueryTests : IClassFixture<VertegenwoordigersPerVCodeQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;
    private readonly Fixture _fixture;

    public VertegenwoordigersPerVCodeQueryTests(VertegenwoordigersPerVCodeQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public async ValueTask Returns_Matching_Doc_When_VCode_Matches()
    {
        await ClearAsync();

        var vertegenwoordigersPerVCodeDocs = _fixture
                                            .CreateMany<VertegenwoordigersPerVCodeDocument>()
                                            .ToList();

        _session.StoreObjects(vertegenwoordigersPerVCodeDocs);
        await _session.SaveChangesAsync();

        var query = new VertegenwoordigersPerVCodeQuery(_session);

        var vCode = vertegenwoordigersPerVCodeDocs[1].VCode;
        var actual = await query.ExecuteAsync(new VertegenwoordigersPerVCodeQueryFilter(vCode: vCode), CancellationToken.None);

        actual.Should().BeEquivalentTo([vertegenwoordigersPerVCodeDocs[1]]);
    }

    [Fact]
    public async ValueTask Returns_Matching_Doc_When_Status_Matches()
    {
        await ClearAsync();

        var status = VertegenwoordigerKszStatus.Bevestigd;

        var doc1 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();
        doc1.VertegenwoordigersData = doc1.VertegenwoordigersData.Append(new VertegenwoordigerData(_fixture.Create<int>(), status)).ToArray();
        var doc2 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();
        doc2.VertegenwoordigersData = doc2.VertegenwoordigersData.Append(new VertegenwoordigerData(_fixture.Create<int>(), status)).ToArray();
        var doc3 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();

        _session.StoreObjects([doc1, doc2, doc3]);
        await _session.SaveChangesAsync();

        var query = new VertegenwoordigersPerVCodeQuery(_session);

        var actual = await query.ExecuteAsync(new VertegenwoordigersPerVCodeQueryFilter(status: status), CancellationToken.None);

        actual.Should().BeEquivalentTo([doc1, doc2]);
    }

    [Fact]
    public async ValueTask Returns_Matching_Doc_When_Status_And_VCode_Matches()
    {
        await ClearAsync();

        var status = VertegenwoordigerKszStatus.Bevestigd;

        var doc1 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();
        doc1.VertegenwoordigersData = doc1.VertegenwoordigersData.Append(new VertegenwoordigerData(_fixture.Create<int>(), status)).ToArray();
        var doc2 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();
        doc2.VertegenwoordigersData = doc2.VertegenwoordigersData.Append(new VertegenwoordigerData(_fixture.Create<int>(), status)).ToArray();
        var doc3 = _fixture.Create<VertegenwoordigersPerVCodeDocument>();

        _session.StoreObjects([doc1, doc2, doc3]);
        await _session.SaveChangesAsync();

        var query = new VertegenwoordigersPerVCodeQuery(_session);

        var vCode = doc1.VCode;
        var actual = await query.ExecuteAsync(new VertegenwoordigersPerVCodeQueryFilter(status: status, vCode: vCode), CancellationToken.None);

        actual.Should().BeEquivalentTo([doc1]);
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync();
    }

    private async Task ClearAsync()
    {
        _session.DeleteWhere<VertegenwoordigersPerVCodeDocument>(_ => true);
        await _session.SaveChangesAsync();
    }
}
