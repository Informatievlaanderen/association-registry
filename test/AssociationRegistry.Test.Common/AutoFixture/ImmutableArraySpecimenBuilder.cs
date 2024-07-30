namespace AssociationRegistry.Test.Common.AutoFixture;

using global::AutoFixture.Kernel;
using System.Collections.Immutable;

public class ImmutableArraySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (request is not Type t)
            return new NoSpecimen();

        var typeArguments = t.GetGenericArguments();

        if (typeArguments.Length != 1)
            return new NoSpecimen();

        if (typeof(ImmutableArray<>) != t.GetGenericTypeDefinition())
            return new NoSpecimen();

        dynamic list = context.Resolve(typeof(IList<>).MakeGenericType(typeArguments));

        return ImmutableArray.ToImmutableArray(list);
    }
}
