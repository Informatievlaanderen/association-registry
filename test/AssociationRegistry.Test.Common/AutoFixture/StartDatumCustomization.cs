namespace AssociationRegistry.Test.Common.AutoFixture;

using global::AutoFixture.Kernel;
using System.Reflection;

public class StartDatumCustomization : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property &&
            property.PropertyType == typeof(DateOnly) &&
            property.Name.Equals("StartDatum", StringComparison.OrdinalIgnoreCase))
        {
            return DateOnly.FromDateTime(DateTime.Now.AddDays(-Random.Shared.Next(1, 365)));
        }

        return new NoSpecimen();
    }
}

