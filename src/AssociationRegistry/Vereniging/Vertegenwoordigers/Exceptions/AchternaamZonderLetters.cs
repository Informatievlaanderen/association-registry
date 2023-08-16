namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
