namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
