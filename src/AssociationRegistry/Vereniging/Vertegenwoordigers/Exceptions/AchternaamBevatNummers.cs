namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AchternaamBevatNummers : DomainException
{
    public AchternaamBevatNummers() : base(ExceptionMessages.AchternaamBevatNummers)
    {
    }

    protected AchternaamBevatNummers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
