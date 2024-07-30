namespace AssociationRegistry.Test.Common.Extensions;

using System.Text;

public static class StringContentExtensions
{
    public static StringContent AsJsonContent(this string jsonContent)
        => new(
            jsonContent,
            Encoding.UTF8,
            mediaType: "application/json");
}
