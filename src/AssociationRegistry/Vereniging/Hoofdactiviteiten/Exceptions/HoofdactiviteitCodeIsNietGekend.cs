namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
