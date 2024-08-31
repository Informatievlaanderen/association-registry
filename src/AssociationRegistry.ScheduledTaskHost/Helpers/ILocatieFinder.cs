namespace AssociationRegistry.ScheduledTaskHost.Helpers;

using AssociationRegistry.Admin.Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] adresIds);
}
