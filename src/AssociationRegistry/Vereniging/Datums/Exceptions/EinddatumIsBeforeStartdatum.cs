namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class EinddatumIsBeforeStartdatum : DomainException
{
    public EinddatumIsBeforeStartdatum() : base(ExceptionMessages.EinddatumIsBeforeStartdatum)
    {
    }

    protected EinddatumIsBeforeStartdatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
