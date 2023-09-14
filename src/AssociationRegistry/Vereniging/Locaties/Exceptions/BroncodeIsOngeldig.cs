namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
