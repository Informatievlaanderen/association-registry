namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.HeradresseerLocaties;
using Grar.Models;
using Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    Task<LocatiesVolgensVCode[]> FindLocaties(params int[] adresIds);
}

public class LocatiesVolgensVCode
{
    public string VCode { get; set; }

    public List<LocatieIdWithAdresId> LocatiesMetAdresId { get; set; }
}
