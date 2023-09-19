namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Nest;
using System;
using System.Linq;

public static class SearchVerenigingenExtensions
{
    public static SearchDescriptor<T> ParseSort<T>(
        this SearchDescriptor<T> source,
        string? sort,
        Func<SortDescriptor<T>, SortDescriptor<T>> defaultSort) where T : class
    {
        if (string.IsNullOrWhiteSpace(sort))
            return source.Sort(defaultSort);

        return source.Sort(_ => SortDescriptor<T>(sort));
    }

    private static SortDescriptor<T> SortDescriptor<T>(string sort) where T : class
    {
        var sortParts = sort.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .ToArray();

        var sortDescriptor = new SortDescriptor<T>();

        foreach (var sortPart in sortParts)
        {
            var descending = sortPart.StartsWith("-");
            var part = descending ? sortPart.Substring(1) : sortPart;
            sortDescriptor.Field(part + ".keyword", descending ? SortOrder.Descending : SortOrder.Ascending);
        }

        return sortDescriptor;
    }
}
