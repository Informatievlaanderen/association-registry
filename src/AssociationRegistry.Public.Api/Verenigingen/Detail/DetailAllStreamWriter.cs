namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Schema.Detail;
using System.Text;

public class DetailAllStreamWriter : IDetailAllStreamWriter
{
    private readonly IDetailAllConverter _converter;

    public DetailAllStreamWriter(IDetailAllConverter converter)
    {
        _converter = converter;
    }

    public async Task<MemoryStream> WriteAsync(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken)
    {
        await using var inputStream = new MemoryStream();
        await using var writer = new StreamWriter(inputStream, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            var verenigingAsJson = _converter.SerializeToJson(vereniging);

            await writer.WriteLineAsync(verenigingAsJson);
        }

        await writer.FlushAsync(cancellationToken);
        inputStream.Position = 0;

        var ms = new MemoryStream();
        await inputStream.CopyToAsync(ms, cancellationToken);

        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}
