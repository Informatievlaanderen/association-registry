namespace AssociationRegistry.Test.Public.Api.Controller;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class PubliekGetAllFixture : IAsyncLifetime
{
    public readonly string RedirectUrl = "https://localhost:4566";
    private readonly Fixture _fixture;
    public DetailVerenigingenController Controller { get; private set; }
    public Mock<IDetailAllS3Client> S3Client { get; init; }
    public Mock<IDetailAllStreamWriter> StreamWriter { get; init; }
    public Mock<IPubliekVerenigingenDetailAllQuery> Query { get; init; }

    public IAsyncEnumerable<PubliekVerenigingDetailDocument> Data { get; set; }

    public MemoryStream Stream { get; set; }

    public PubliekGetAllFixture()
    {
        _fixture = new Fixture().CustomizePublicApi();
        Data = GetData();

        Query = setupQueryMock(new Mock<IPubliekVerenigingenDetailAllQuery>());
        StreamWriter = SetupStreamWriterMock(new Mock<IDetailAllStreamWriter>());
        S3Client = SetupS3ClientMock(new Mock<IDetailAllS3Client>());
    }

    private Mock<IPubliekVerenigingenDetailAllQuery>? setupQueryMock(Mock<IPubliekVerenigingenDetailAllQuery> mock)
    {
        mock.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Data);

        return mock;
    }

    private async IAsyncEnumerable<PubliekVerenigingDetailDocument> GetData()
    {
        var docs = _fixture.CreateMany<PubliekVerenigingDetailDocument>();

        foreach (var doc in docs)
        {
            yield return doc;
        }
    }

    public async Task InitializeAsync()
    {
        Controller = new DetailVerenigingenController(new AppSettings());
        Response = await Controller.GetAll(Query.Object, StreamWriter.Object, S3Client.Object, CancellationToken.None);
    }

    protected Mock<IDetailAllStreamWriter> SetupStreamWriterMock(Mock<IDetailAllStreamWriter> mock)
    {
        mock.Setup(s => s.WriteAsync(Data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stream);

        return mock;
    }

    protected Mock<IDetailAllS3Client> SetupS3ClientMock(Mock<IDetailAllS3Client> mock)
    {
        mock.Setup(x => x.GetPreSignedUrlAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(RedirectUrl);

        return mock;
    }

    public IActionResult Response { get; private set; }

    public async Task DisposeAsync()
    {
    }
}
