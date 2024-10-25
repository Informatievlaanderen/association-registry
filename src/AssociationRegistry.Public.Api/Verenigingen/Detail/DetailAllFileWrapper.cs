namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.ConfigurationBindings;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

// public class DetailAllFileWrapper : IDetailAllFileWrapper
// {
//     private readonly IAmazonS3 _s3Client;
//     private readonly string _bucketName;
//     private readonly string _key;
//
//     public DetailAllFileWrapper(IAmazonS3 s3Client, AppSettings appSettings)
//     {
//         _s3Client = s3Client;
//         _bucketName = appSettings.Publiq.BucketName;
//         _key = appSettings.Publiq.Key;
//     }
//
//     public async Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken)
//         => await _s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
//         {
//             BucketName = _bucketName,
//             Key = _key,
//             Expires = DateTime.Now.AddMinutes(5),
//             Verb = HttpVerb.GET,
//         });
//
//     public async Task PutAsync(object[] data, CancellationToken cancellationToken)
//     {
//
//         await using var inputStream = new MemoryStream();
//         await using var writer = new StreamWriter(inputStream, Encoding.UTF8);
//
//         foreach (var vereniging in data)
//         {
//             if (IsTeVerwijderenVereniging(vereniging))
//             {
//                 var teVerwijderenVereniging =
//                     JsonConvert.SerializeObject(
//                         new DetailAllFile.TeVerwijderenVereniging
//                         {
//                             Vereniging = new DetailAllFile.TeVerwijderenVereniging.TeVerwijderenVerenigingData
//                             {
//                                 VCode = vereniging.VCode,
//                                 TeVerwijderen = true,
//                                 DeletedAt = vereniging.DatumLaatsteAanpassing,
//                             },
//                         },
//                         _serializerSettings);
//
//                 await writer.WriteLineAsync(teVerwijderenVereniging);
//
//                 continue;
//             }
//
//             var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, _appSettings);
//             var json = JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
//             await writer.WriteLineAsync(json);
//         }
//
//         stream.Position = 0;
//
//         var request = new PutObjectRequest
//         {
//             BucketName = _bucketName,
//             Key = _key,
//             InputStream = stream,
//             ContentType = MediaTypeNames.Text.Plain,
//         };
//
//         await _s3Client.PutObjectAsync(request, cancellationToken);
//     }
//}
