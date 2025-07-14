namespace AssociationRegistry.Vereniging.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class DatumFormaatIsOngeldig : DomainException
{
    public DatumFormaatIsOngeldig() : base(ExceptionMessages.InvalidDateFormat)
    {
    }

    protected DatumFormaatIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
