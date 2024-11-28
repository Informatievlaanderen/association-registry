namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.HeradresseerLocaties;
using Schema.Detail;

public interface ILocatieFinder
{
    Task<IEnumerable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    //Task<IEnumerable<TeHeradresserenLocatiesMessage>> FindLocaties(params int[] adresIds);
}
