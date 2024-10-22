namespace AssociationRegistry.Test.Public.Api.Controller;

using AssociationRegistry.Framework;
using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class PubliekGetAllFixture : IAsyncLifetime
{
    public readonly string RedirectUrl = "https://localhost:4566";
    public DetailVerenigingenController Controller { get; private set; }
    public Mock<IDetailAllS3Client> S3Client { get; init; }
    public Mock<IDetailAllStreamWriter> StreamWriter { get; init; }
    public Mock<IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>> Query { get; init; }

    public PubliekGetAllFixture()
    {
        Query = new Mock<IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>>();
        StreamWriter = SetupStreamWriterMock(new Mock<IDetailAllStreamWriter>());
        S3Client = SetupS3ClientMock(new Mock<IDetailAllS3Client>());
    }

    public async Task InitializeAsync()
    {
        Controller = new DetailVerenigingenController(new AppSettings());
        Response = await Controller.GetAll(Query.Object, StreamWriter.Object, S3Client.Object, CancellationToken.None);
    }

    protected Mock<IDetailAllStreamWriter> SetupStreamWriterMock(Mock<IDetailAllStreamWriter> mock)
    {
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
