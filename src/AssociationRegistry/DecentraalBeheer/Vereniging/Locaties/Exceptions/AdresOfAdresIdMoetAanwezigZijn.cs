namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class AdresOfAdresIdMoetAanwezigZijn : DomainException
{
    public AdresOfAdresIdMoetAanwezigZijn() : base(ExceptionMessages.MissingAdres)
    {
    }

    protected AdresOfAdresIdMoetAanwezigZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
