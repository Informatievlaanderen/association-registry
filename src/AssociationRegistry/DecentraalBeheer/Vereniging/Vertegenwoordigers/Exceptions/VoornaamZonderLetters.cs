namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
