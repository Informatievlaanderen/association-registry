namespace AssociationRegistry.Activiteiten.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownHoofdactiviteitCode: DomainException
{
    public UnknownHoofdactiviteitCode() : base("De opgegeven code is niet gekend.")
    {
    }

    protected UnknownHoofdactiviteitCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
