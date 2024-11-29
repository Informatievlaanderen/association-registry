namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Grar.HeradresseerLocaties;

public interface ITeHeradresserenLocatiesFinder
{
    Task<TeHeradresserenLocatiesMessage[]> Find(int addressPersistentLocalId);
}
