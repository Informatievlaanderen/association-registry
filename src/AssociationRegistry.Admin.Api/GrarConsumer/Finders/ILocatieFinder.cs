namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Schema.Detail;

public interface ILocatieFinder
{
    Task<IQueryable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    Task<LocatieLookupData[]> FindLocaties(params int[] adresIds);
}
