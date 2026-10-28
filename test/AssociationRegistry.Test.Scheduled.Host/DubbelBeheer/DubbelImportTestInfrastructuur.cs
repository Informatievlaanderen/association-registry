namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;

/// <summary>Eenvoudige in-memory S3 op basis van Moq, enkel de operaties die de import gebruikt.</summary>
public class InMemoryS3
{
    public Dictionary<string, byte[]> Objecten { get; } = new();

    public IAmazonS3 Client { get; }

    public InMemoryS3()
    {
        var mock = new Mock<IAmazonS3>();

        mock.Setup(x => x.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
                new ListObjectsV2Response
                {
                    S3Objects = Objecten.Keys.Select(key => new S3Object { Key = key }).ToList(),
                }
            );

        mock.Setup(x => x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (string _, string key, CancellationToken _) =>
                    new GetObjectResponse { Key = key, ResponseStream = new MemoryStream(Objecten[key]) }
            );

        mock.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (string _, string key, CancellationToken _) =>
                {
                    Objecten.Remove(key);

                    return new DeleteObjectResponse();
                }
            );

        Client = mock.Object;
    }

    public void VoegBestandToe(string key, string inhoud) => Objecten[key] = Encoding.UTF8.GetBytes(inhoud);
}
