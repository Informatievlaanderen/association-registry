namespace AssociationRegistry.Admin.Api.GrarConsumer;

using Grar.HeradresseerLocaties;

public interface ITeHeradresserenLocatiesFinder
{
    Task<TeHeradresserenLocatiesMessage[]> Find(int addressPersistentLocalId);
}
