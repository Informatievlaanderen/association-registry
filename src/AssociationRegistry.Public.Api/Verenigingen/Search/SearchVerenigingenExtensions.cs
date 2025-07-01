namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Nest;
using System;
using System.Linq;

public static class SearchVerenigingenExtensions
{
    private const string ElasticsearchScoreField = "_score";
    private const string UserScoreField = "score";
    private const string KeywordSuffix = ".keyword";

    public static SearchDescriptor<T> ParseSort<T>(
        this SearchDescriptor<T> source,
        string? sort,
        Func<SortDescriptor<T>, SortDescriptor<T>> defaultSort,
        ITypeMapping mapping) where T : class
    {
        if (string.IsNullOrWhiteSpace(sort))
            return source.Sort(defaultSort);

        var customSortDescriptor = CreateSortDescriptor<T>(sort, mapping);
        return source.Sort(sortDescriptor => customSortDescriptor.ThenBy(defaultSort));
    }

    private static SortDescriptor<T> CreateSortDescriptor<T>(string sort, ITypeMapping mapping) where T : class
    {
        var sortParts = ParseSortString(sort);
        var sortDescriptor = new SortDescriptor<T>();

        foreach (var (field, isDescending) in sortParts)
        {
            var resolvedField = ResolveFieldName(field, mapping);
            var sortOrder = isDescending ? SortOrder.Descending : SortOrder.Ascending;
            sortDescriptor.Field(resolvedField, sortOrder);
        }

        return sortDescriptor;
    }

    private static (string field, bool isDescending)[] ParseSortString(string sort)
    {
        return sort.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(part => part.Trim())
                  .Where(part => !string.IsNullOrEmpty(part))
                  .Select(ParseSortPart)
                  .ToArray();
    }

    private static (string field, bool isDescending) ParseSortPart(string sortPart)
    {
        var isDescending = sortPart.StartsWith('-');
        var field = isDescending ? sortPart[1..] : sortPart;
        return (field, isDescending);
    }

    private static string ResolveFieldName(string field, ITypeMapping mapping)
    {
        // Handle special case for score field
        if (string.Equals(field, UserScoreField, StringComparison.OrdinalIgnoreCase))
            return ElasticsearchScoreField;

        var fieldType = GetFieldType(mapping, field);

        return fieldType switch
        {
            "integer" => field,
            "keyword" => field,
            "date" => field,
            "boolean" => field,
            _ => $"{field}{KeywordSuffix}" // Default to .keyword for text fields
        };
    }

    private static string? GetFieldType(ITypeMapping mapping, string fieldPath)
    {
        var pathSegments = fieldPath.Split('.');
        return InspectPropertyType(mapping.Properties, pathSegments, 0);
    }

    private static string? InspectPropertyType(IProperties properties, string[] pathSegments, int currentIndex)
    {
        if (currentIndex >= pathSegments.Length || !properties.ContainsKey(pathSegments[currentIndex]))
            return null;

        var currentProperty = properties[pathSegments[currentIndex]];

        // If we've reached the target property, return its type
        if (currentIndex == pathSegments.Length - 1)
            return currentProperty.Type;

        // If it's an object property, recurse into its properties
        if (currentProperty is ObjectProperty objectProperty && objectProperty.Properties != null)
            return InspectPropertyType(objectProperty.Properties, pathSegments, currentIndex + 1);

        return null;
    }

    private static SortDescriptor<T> ThenBy<T>(this SortDescriptor<T> first, Func<SortDescriptor<T>, SortDescriptor<T>> second)
        where T : class
        => second(first);
}
