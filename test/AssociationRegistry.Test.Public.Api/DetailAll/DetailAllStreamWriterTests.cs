namespace AssociationRegistry.Test.Public.Api.DetailAll;

using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Api.Verenigingen.DetailAll;
using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Public.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class DetailAllStreamWriterTests
{
    private readonly Fixture _fixture;

    public DetailAllStreamWriterTests()
    {
        _fixture = new Fixture().CustomizePublicApi();
    }

    [Fact]
    public async Task WithEmptyData_ReturnsEmptyStream()
    {
        var sut = new DetailAllStreamWriter(Mock.Of<IDetailAllConverter>());

        var stream = await sut.WriteAsync(GetData([]), CancellationToken.None);

        using var reader = new StreamReader(stream);

        var streamData = await reader.ReadToEndAsync();
        streamData.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task WithData_ReturnsStreamWithData()
    {
        var publiekVerenigingDetailDocument = _fixture.Create<PubliekVerenigingDetailDocument>();
        var mock = new Mock<IDetailAllConverter>();

        var mappedJson = _fixture.Create<string>();

        mock.Setup(s => s.SerializeToJson(publiekVerenigingDetailDocument))
            .Returns(mappedJson);

        var sut = new DetailAllStreamWriter(mock.Object);

        var stream = await sut.WriteAsync(GetData([publiekVerenigingDetailDocument]), CancellationToken.None);

        using var reader = new StreamReader(stream);

        var streamData = await reader.ReadToEndAsync();
        streamData.Should().BeEquivalentTo(mappedJson + Environment.NewLine);
    }

    private async IAsyncEnumerable<PubliekVerenigingDetailDocument> GetData(IEnumerable<PubliekVerenigingDetailDocument> data)
    {
        var docs = data;

        foreach (var doc in docs)
        {
            yield return doc;
        }
    }
}


