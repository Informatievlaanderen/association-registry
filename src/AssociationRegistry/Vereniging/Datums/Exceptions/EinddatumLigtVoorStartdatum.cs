namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class EinddatumLigtVoorStartdatum : DomainException
{
    public EinddatumLigtVoorStartdatum() : base(ExceptionMessages.EinddatumIsBeforeStartdatum)
    {
    }

    protected EinddatumLigtVoorStartdatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
