namespace AssociationRegistry.Test.Framework.Customizations;

using AutoFixture;
using AutoFixture.Kernel;
using Marten.Events;
using NodaTime;

public class TestEventSpecimenBuilder : ISpecimenBuilder
{
    private readonly Type _testEventType;

    public TestEventSpecimenBuilder(Type testEventType)
    {
        _testEventType = testEventType;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (request is not Type t)
            return new NoSpecimen();

        var typeArguments = t.GetGenericArguments();

        if (typeArguments.Length != 1)
            return new NoSpecimen();

        if (_testEventType != t.GetGenericTypeDefinition())
            return new NoSpecimen();

        var @event = context.Resolve(typeArguments.Single());
        var instance = (IEvent)Activator.CreateInstance(t, @event, context.Create<string>(), context.Create<Instant>())!;
        instance.Version = context.Create<long>();
        instance.Sequence = context.Create<long>();

        return instance;
    }
}
