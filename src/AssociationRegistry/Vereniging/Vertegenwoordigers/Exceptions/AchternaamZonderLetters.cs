namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AchternaamZonderLetters : DomainException
{
    public AchternaamZonderLetters() : base("Achternaam moet een letter bevatten.")
    {
    }

    protected AchternaamZonderLetters(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
