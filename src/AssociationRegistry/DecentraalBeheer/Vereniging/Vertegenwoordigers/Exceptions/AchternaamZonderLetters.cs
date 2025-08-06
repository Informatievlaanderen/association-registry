namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class AchternaamZonderLetters : DomainException
{
    public AchternaamZonderLetters() : base(ExceptionMessages.AchternaamZonderLetters)
    {
    }

    protected AchternaamZonderLetters(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
