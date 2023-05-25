namespace AssociationRegistry;

public interface IHydrate<in T>
{
    void Hydrate(T obj);
}
