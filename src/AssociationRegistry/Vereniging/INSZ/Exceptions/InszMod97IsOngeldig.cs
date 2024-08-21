namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
