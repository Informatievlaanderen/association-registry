namespace AssociationRegistry.Test.Public.Api.UnitTests;

using System.Collections.Immutable;
using AutoFixture;
using AutoFixture.Kernel;

public class VerenigingenFixture : Fixture
{
    private static Random _random = new();

    public VerenigingenFixture()
    {
        Customizations.Add(new ImmutableListSpecimenBuilder());
        Customizations.Add(new ImmutableArraySpecimenBuilder());
        Customizations.Add(new DateOnlySpecimenBuilder());
    }


    internal class ImmutableListSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var t = request as Type;
            if (t == null)
            {
                return new NoSpecimen();
            }

            var typeArguments = t.GetGenericArguments();
            if (typeArguments.Length != 1 || typeof(ImmutableList<>) != t.GetGenericTypeDefinition())
            {
                return new NoSpecimen();
            }

            dynamic list = context.Resolve(typeof(IList<>).MakeGenericType(typeArguments));

            return ImmutableList.ToImmutableList(list);
        }
    }

    internal class ImmutableArraySpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var t = request as Type;
            if (t == null)
            {
                return new NoSpecimen();
            }

            var typeArguments = t.GetGenericArguments();
            if (typeArguments.Length != 1 || typeof(ImmutableArray<>) != t.GetGenericTypeDefinition())
            {
                return new NoSpecimen();
            }

            dynamic list = context.Resolve(typeof(IList<>).MakeGenericType(typeArguments));

            return ImmutableArray.ToImmutableArray(list);
        }
    }

    internal class DateOnlySpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (request is not Type t || t != typeof(DateOnly))
            {
                return new NoSpecimen();
            }

            dynamic dateTime = context.Resolve(typeof(DateTime));

            return DateOnly.FromDateTime(dateTime);
        }
    }
}

