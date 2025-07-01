namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Nest;
using System;
using System.Linq;

public static class SearchVerenigingenExtensions
{
    private const string Score = "_score";

    public static SearchDescriptor<T> ParseSort<T>(
        this SearchDescriptor<T> source,
        string? sort,
        Func<SortDescriptor<T>, SortDescriptor<T>> defaultSort,
        ITypeMapping mapping) where T : class
    {
        if (string.IsNullOrWhiteSpace(sort))
            return source.Sort(defaultSort);

        if (sort.Contains("score"))
            sort = sort.Replace("score", Score);

        return source.Sort(_ => SortDescriptor<T>(sort, mapping).ThenBy(defaultSort));
    }

    private static SortDescriptor<T> SortDescriptor<T>(string sort, ITypeMapping mapping) where T : class
    {
        var sortParts = sort.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .ToArray();

        var sortDescriptor = new SortDescriptor<T>();

        foreach (var sortPart in sortParts)
        {
            var descending = sortPart.StartsWith("-");
            var part = descending ? sortPart.Substring(1) : sortPart;
            var isKeyword = IsKeyword(mapping, part);

            var fieldType = InspectPropertyType(mapping.Properties, part.Split('.'), 0);

            var resolvedField = fieldType switch
            {
                "integer" => part,
                _ => $"{part}{(isKeyword ? "" : ".keyword")}",            // others (keyword, integer, etc.) should not get .keyword
            };

            sortDescriptor.Field(resolvedField, descending ? SortOrder.Descending : SortOrder.Ascending);
        }

        return sortDescriptor;
    }

    private static bool IsKeyword(ITypeMapping mapping, string field)
        => field != Score &&
            InspectPropertyType(mapping.Properties, field.Split('.'), currentIndex: 0) == "keyword";

    private static string InspectPropertyType(IProperties properties, string[] pathSegments, int currentIndex)
    {
        if (currentIndex < pathSegments.Length && properties.ContainsKey(pathSegments[currentIndex]))
        {
            var currentProperty = properties[pathSegments[currentIndex]];

            if (currentIndex == pathSegments.Length - 1)
                // We've reached the desired property
                return currentProperty.Type;

            if (currentProperty is ObjectProperty objectProperty)
                // We need to delve deeper into the object properties
                return InspectPropertyType(objectProperty.Properties, pathSegments, currentIndex + 1);
        }

        // The desired property or path wasn't found
        return null;
    }

    private static SortDescriptor<T> ThenBy<T>(this SortDescriptor<T> first, Func<SortDescriptor<T>, SortDescriptor<T>> second)
        where T : class
        => second(first);
}
