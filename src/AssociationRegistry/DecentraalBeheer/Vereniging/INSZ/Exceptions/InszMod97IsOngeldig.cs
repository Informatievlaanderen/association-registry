namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class InszMod97IsOngeldig : DomainException
{
    public InszMod97IsOngeldig() : base(ExceptionMessages.InvalidInszMod97)
    {
    }

    protected InszMod97IsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
