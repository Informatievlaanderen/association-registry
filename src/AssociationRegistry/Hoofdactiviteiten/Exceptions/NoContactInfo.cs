namespace AssociationRegistry.Hoofdactiviteiten.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownHoofdactiviteitCode : DomainException
{
    public UnknownHoofdactiviteitCode(string unknownCode) : base($"De opgegeven hoofdactiviteit is niet gekend: \"{unknownCode}\"")
    {
    }

    protected UnknownHoofdactiviteitCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
