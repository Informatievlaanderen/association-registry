namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class VoornaamZonderLetters : DomainException
{
    public VoornaamZonderLetters() : base(ExceptionMessages.VoornaamZonderLetters)
    {
    }

    protected VoornaamZonderLetters(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
