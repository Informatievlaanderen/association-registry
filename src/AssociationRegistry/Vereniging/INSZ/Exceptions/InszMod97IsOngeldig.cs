namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
