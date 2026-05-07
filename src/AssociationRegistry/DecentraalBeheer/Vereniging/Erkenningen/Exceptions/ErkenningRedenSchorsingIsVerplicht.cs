namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningRedenSchorsingIsVerplicht : DomainException
{
    public ErkenningRedenSchorsingIsVerplicht()
        : base(ExceptionMessages.ErkenningRedenSchorsingVerplicht) { }

    protected ErkenningRedenSchorsingIsVerplicht(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
