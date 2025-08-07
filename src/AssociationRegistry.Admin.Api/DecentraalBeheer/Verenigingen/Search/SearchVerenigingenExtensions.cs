namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SearchVerenigingenExtensions
{
    /// <summary>
    /// Parses a comma-separated sort string (e.g., "-name,address.street") into Elasticsearch SortOptions.
    /// - Leading '-' => descending; otherwise ascending.
    /// - If the leaf mapping is TextProperty => append ".keyword" for sortable field.
    /// - If the path crosses a NestedProperty => add Nested sort metadata with the first nested parent path.
    /// Falls back to the original field path when the mapping can't be resolved.
    /// Parsed sorts are concatenated before the provided defaultSort.
    /// </summary>
    public static List<SortOptions> ParseSort(
        string? sort,
        List<SortOptions> defaultSort,
        TypeMapping mapping)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return defaultSort;

        var parsed = ParseSortInternal(sort, mapping);
        return parsed.Concat(defaultSort).ToList();
    }

    private static IEnumerable<SortOptions> ParseSortInternal(string sort, TypeMapping mapping)
    {
        foreach (var part in SplitSortParts(sort))
        {
            var isDesc = part.StartsWith("-");
            var rawFieldPath = isDesc ? part[1..] : part;

            if (string.IsNullOrWhiteSpace(rawFieldPath))
                continue;

            var segments = SplitPathSegments(rawFieldPath);
            if (segments.Length == 0)
                continue;

            // Determine whether we need ".keyword" and whether we need nested sort metadata.
            var leafProp = FindLeafProperty(mapping.Properties, segments);
            var resolvedFieldPath = ResolveFieldPathForSort(rawFieldPath, leafProp);
            var nested = BuildNestedSortIfNeeded(mapping.Properties, segments);

            yield return MakeFieldSort(resolvedFieldPath, isDesc, nested);
        }
    }

    private static IEnumerable<string> SplitSortParts(string sort) =>
        sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string[] SplitPathSegments(string path) =>
        path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static SortOptions MakeFieldSort(string fieldPath, bool isDesc, NestedSortValue? nested) =>
        new SortOptions
        {
            Field = new FieldSort
            {
                Field = fieldPath,                      // implicit conversion to Field
                Order = isDesc ? SortOrder.Desc : SortOrder.Asc,
                Nested = nested
            }
        };

    /// <summary>
    /// If the leaf property is a TextProperty, sort on the ".keyword" subfield; otherwise keep as-is.
    /// If the leaf property can't be determined (null), keep the original field path (no guessing).
    /// </summary>
    private static string ResolveFieldPathForSort(string originalFieldPath, IProperty? leafProp) =>
        leafProp is TextProperty ? $"{originalFieldPath}.keyword" : originalFieldPath;

    /// <summary>
    /// Returns NestedSortValue with the first nested parent path if the path traverses a NestedProperty; otherwise null.
    /// </summary>
    private static NestedSortValue? BuildNestedSortIfNeeded(Properties props, string[] segments)
    {
        var nestedPath = GetFirstNestedParentPath(props, segments);
        if (string.IsNullOrEmpty(nestedPath))
            return null;

        return new NestedSortValue { Path = nestedPath };
    }

    /// <summary>
    /// Walks the mapping down the provided path segments and returns the leaf IProperty (if any).
    /// Handles ObjectProperty and NestedProperty properly.
    /// </summary>
    private static IProperty? FindLeafProperty(Properties props, string[] segments, int index = 0)
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
                => FindLeafProperty(op.Properties, segments, index + 1),

            NestedProperty np when np.Properties is not null
                => FindLeafProperty(np.Properties, segments, index + 1),

            _ => null
        };
    }

    /// <summary>
    /// Returns the dot-path to the first NestedProperty encountered while walking the path.
    /// If none is encountered, returns null.
    /// </summary>
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
