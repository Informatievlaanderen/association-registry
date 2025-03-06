namespace AssociationRegistry.Test.Admin.Api.Framework.Helpers;

using System.Net.Http;
using System.Text;

public static class IntegrationTestHelpers
{
    public static StringContent AsJsonContent(this string jsonContent)
        => new(
            jsonContent,
            Encoding.UTF8,
            mediaType: "application/json");
}
