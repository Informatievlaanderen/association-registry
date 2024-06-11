namespace AssociationRegistry.Admin.Api.GrarSync;

using Schema.Detail;

public interface ILocatieFinder
{
    public Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string adresId);
}
