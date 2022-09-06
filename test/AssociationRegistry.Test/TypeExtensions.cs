namespace AssociationRegistry.Test;

public static class TypeExtensions
{
    /// <summary>
    /// usage: put a json-file next to the type
    /// it should have the same name as the type, but with an added suffix
    /// like this: typename_suffix.json (mind the underscore)
    /// then you pass "suffix" into the filenameSuffix parameter of this method
    /// </summary>
    /// <returns>the contents of the embedded csv that matches the calculated filename</returns>

    public static string GetAssociatedResourceJson(this Type type, string filenameSuffix)
        => type.GetResourceString(suffix: filenameSuffix, extension: "json");

    private static string GetResourceString(this Type type, string? suffix = null, string? extension = null)
    {
        var resourceName = type.FullName!;

        if (!string.IsNullOrWhiteSpace(suffix))
            resourceName = $"{resourceName}_{suffix}";

        if (!string.IsNullOrWhiteSpace(extension))
            resourceName = $"{resourceName}.{extension}";

        return type.Assembly.GetResourceString(resourceName);
    }
}
