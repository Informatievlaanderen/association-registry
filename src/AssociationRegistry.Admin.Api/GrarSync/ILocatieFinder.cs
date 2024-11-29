namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.Models;
using Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    Task<LocatieLookupData[]> FindLocaties(params int[] adresIds);
}
