namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.HeradresseerLocaties;

public interface ITeHeradresserenLocatiesFinder
{
    Task<TeHeradresserenLocatiesMessage[]> Find(int addressPersistentLocalId);
}
