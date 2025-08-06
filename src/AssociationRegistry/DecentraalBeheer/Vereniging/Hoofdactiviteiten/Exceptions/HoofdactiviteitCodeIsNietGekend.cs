namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class HoofdactiviteitCodeIsNietGekend : DomainException
{
    public HoofdactiviteitCodeIsNietGekend(string unknownCode) : base($"De opgegeven hoofdactiviteit is niet gekend: \"{unknownCode}\"")
    {
    }

    protected HoofdactiviteitCodeIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
