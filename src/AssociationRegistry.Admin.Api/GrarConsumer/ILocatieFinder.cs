namespace AssociationRegistry.Admin.Api.GrarConsumer;

using Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    Task<LocatieLookupData[]> FindLocaties(params int[] adresIds);
}
