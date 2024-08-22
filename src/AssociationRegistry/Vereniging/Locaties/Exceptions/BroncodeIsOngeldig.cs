namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class BroncodeIsOngeldig : DomainException
{
    public BroncodeIsOngeldig() : base(ExceptionMessages.InvalidBroncode)
    {
    }

    protected BroncodeIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
