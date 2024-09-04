// namespace AssociationRegistry.Test.PowerBi.ExportHost;
//
// using Admin.Schema.PowerBiExport;
// using AssociationRegistry.PowerBi.ExportHost;
// using AssociationRegistry.PowerBi.ExportHost.Writers;
// using AutoFixture;
// using Common.AutoFixture;
// using FluentAssertions;
// using System.Text;
// using Xunit;
//
// public class ContactgegevensExportTests
// {
//     [Fact]
//     public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
//     {
//
//         var fixture = new Fixture().CustomizeDomain();
//
//         var docs = fixture.CreateMany<PowerBiExportDocument>();
//
//         var content = await GenerateCsv(docs);
//         var stringBuilder = new StringBuilder();
//         stringBuilder.Append("beschrijving,bron,contactgegevenId,contactgegevenType,isPrimair,vCode,waarde\r\n");
//
//         foreach (var doc in docs)
//         {
//             foreach (var contactgegeven in doc.Contactgegevens)
//             {
//                 stringBuilder.Append($"{contactgegeven.Beschrijving},{contactgegeven.Bron},{contactgegeven.ContactgegevenId},{contactgegeven.Contactgegeventype},{contactgegeven.IsPrimair},{doc.VCode},{contactgegeven.Waarde}\r\n");
//             }
//         }
//
//         content.Should().BeEquivalentTo(stringBuilder.ToString());
//     }
//
//     private static async Task<string> GenerateCsv(IEnumerable<PowerBiExportDocument> docs)
//     {
//         var exporter = new PowerBiDocumentExporter();
//
//         var exportStream = await exporter.Export(docs, new ContactgegevensRecordWriter());
//
//         using var reader = new StreamReader(exportStream, Encoding.UTF8);
//
//         var content = await reader.ReadToEndAsync();
//
//         return content;
//     }
// }
//
//
