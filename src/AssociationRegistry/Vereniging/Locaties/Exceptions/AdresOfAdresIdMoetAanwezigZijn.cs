namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
