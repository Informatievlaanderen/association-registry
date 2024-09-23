namespace AssociationRegistry.Test.Admin.Api.Queries;

using Common.Framework;
using FluentAssertions;
using Marten;
using Public.Api.Infrastructure.Extensions;
using Public.Api.Queries;
using Public.Schema.Detail;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PubliekDetailAllQueryTests
{
    [Fact]
    public async Task TestQuery()
    {
        var store = await TestDocumentStoreFactory.Create(nameof(PubliekDetailAllQueryTests));

        await using var session = store.LightweightSession();

        var query = new PubliekDetailAllQuery(session);

        var actual =
            await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().BeEmpty();
    }

    public async Task<List<T>> ConvertToListAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();
        await foreach (var item in asyncEnumerable)
        {
            list.Add(item);
        }
        return list;
    }
}


