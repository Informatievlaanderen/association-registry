namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
