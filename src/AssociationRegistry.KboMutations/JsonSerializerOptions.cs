namespace AssociationRegistry.KboMutations;

using System.Text.Json;

public class JsonSerializerDefaultOptions
{
    public static JsonSerializerOptions Default => new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}