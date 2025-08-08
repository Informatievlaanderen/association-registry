using System;
using System.Collections.Generic;
using System.Linq;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;

namespace AssociationRegistry.Public.Api.Verenigingen.Search;

public static class SearchVerenigingenExtensions
{
    private const string ElasticsearchScoreField = "_score";
    private const string UserScoreField = "score";
    private const string KeywordSuffix = ".keyword";

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

    private static List<SortOptions> ParseSortInternal(string sort, TypeMapping mapping)
    {
        var sortParts = ParseSortString(sort);
        var sortList = new List<SortOptions>();

        foreach (var (field, isDescending) in sortParts)
        {
            // Special case: user 'score' maps to _score
            if (string.Equals(field, UserScoreField, StringComparison.OrdinalIgnoreCase))
            {
                sortList.Add(new SortOptions
                {
                    Field = new FieldSort
                    {
                        Field = ElasticsearchScoreField,
                        Order = isDescending ? SortOrder.Desc : SortOrder.Asc
                    }
                });
                continue;
            }

            var segments = field.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var leafProp = FindProperty(mapping.Properties, segments);

            // Append .keyword only for TextProperty
            var resolvedField = leafProp is TextProperty
                ? field + KeywordSuffix
                : field;

            // If the path traverses a NestedProperty, we must provide Nested on the sort
            var nestedParentPath = GetFirstNestedParentPath(mapping.Properties, segments);

            var fieldSort = new FieldSort
            {
                Field = resolvedField,
                Order = isDescending ? SortOrder.Desc : SortOrder.Asc
            };

            if (!string.IsNullOrEmpty(nestedParentPath))
            {
                fieldSort.Nested = new NestedSortValue { Path = nestedParentPath };
                // When a doc has multiple nested values, choose how to collapse them.
                // Min pairs nicely with ascending; Max with descending.
                fieldSort.Mode = isDescending ? SortMode.Max : SortMode.Min;
            }

            sortList.Add(new SortOptions { Field = fieldSort });
        }

        return sortList;
    }

    private static (string field, bool isDescending)[] ParseSortString(string sort)
        => sort.Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Trim())
               .Where(part => !string.IsNullOrEmpty(part))
               .Select(ParseSortPart)
               .ToArray();

    private static (string field, bool isDescending) ParseSortPart(string sortPart)
    {
        var isDescending = sortPart.StartsWith('-');
        var field = isDescending ? sortPart[1..] : sortPart;
        return (field, isDescending);
    }

    /// <summary>
    /// Walks the mapping and returns the leaf IProperty (if found).
    /// Handles ObjectProperty and NestedProperty.
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

    /// <summary>
    /// Returns the first parent path that is a NestedProperty, e.g. "locaties" for "locaties.postcode".
    /// If the path does not pass through a NestedProperty, returns null.
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
                NestedProperty np => np.Properties,
                _ => null
            };
        }

        return null;
    }
}
