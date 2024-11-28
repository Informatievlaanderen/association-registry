namespace AssociationRegistry.Admin.Api.GrarSync;

using Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
}
