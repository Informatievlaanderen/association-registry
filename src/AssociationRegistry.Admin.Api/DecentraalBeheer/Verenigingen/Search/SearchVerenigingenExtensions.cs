namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SearchVerenigingenExtensions
{
    public static List<SortOptions> ParseSort(
        string? sort,
        List<SortOptions> defaultSort,
        TypeMapping mapping)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return defaultSort;

        return ParseSortInternal(sort, mapping)
            .Concat(defaultSort)
            .ToList();
    }

    private static IEnumerable<SortOptions> ParseSortInternal(string sort, TypeMapping mapping)
    {
        var sortParts = sort
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());

        foreach (var sortPart in sortParts)
        {
            var descending = sortPart.StartsWith("-");
            var fieldPath = descending ? sortPart[1..] : sortPart;

            var segments = fieldPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                continue;

            // Find the leaf property so we know whether to add ".keyword"
            var leafProp = FindProperty(mapping.Properties, segments);

            // If we can’t find it, fall back to the original fieldPath (don’t guess).
            var resolvedFieldPath = leafProp is TextProperty
                ? $"{fieldPath}.keyword"
                : fieldPath;

            // If the path goes through a NestedProperty, Elasticsearch requires nested sort metadata
            var nestedParentPath = GetFirstNestedParentPath(mapping.Properties, segments);
            NestedSortValue? nested = null;

            if (!string.IsNullOrEmpty(nestedParentPath))
            {
                nested = new NestedSortValue
                {
                    Path = nestedParentPath,
                };
            }

            yield return new SortOptions
            {
                Field = new FieldSort
                {
                    Field = resolvedFieldPath,                // implicit conversion to Field
                    Order = descending ? SortOrder.Desc : SortOrder.Asc,
                    Nested = nested
                }
            };
        }
    }

    /// <summary>
    /// Walks the mapping down the provided path segments and returns the leaf IProperty (if any).
    /// Handles ObjectProperty and NestedProperty properly.
    /// </summary>
    private static IProperty? FindProperty(Properties props, string[] segments, int index = 0)
    {
        if (index >= segments.Length)
            return null;

        var name = (PropertyName)segments[index];
        if (!props.TryGetProperty(name, out var prop))
            return null;

        if (index == segments.Length - 1)
            return prop;

        return prop switch
        {
            ObjectProperty op when op.Properties is not null
                => FindProperty(op.Properties, segments, index + 1),

            NestedProperty np when np.Properties is not null
                => FindProperty(np.Properties, segments, index + 1),

            _ => null
        };
    }

    private static string? GetFirstNestedParentPath(Properties props, string[] segments)
    {
        Properties? current = props;
        var pathParts = new List<string>();

        for (int i = 0; i < segments.Length; i++)
        {
            var name = (PropertyName)segments[i];

            if (current is null || !current.TryGetProperty(name, out var prop))
                return null;

            pathParts.Add(segments[i]);

            if (prop is NestedProperty)
                return string.Join('.', pathParts);

            current = prop switch
            {
                ObjectProperty op => op.Properties,
                NestedProperty np => np.Properties, // keep walking; we still return the FIRST nested parent above
                _ => null
            };
        }

        return null;
    }
}
