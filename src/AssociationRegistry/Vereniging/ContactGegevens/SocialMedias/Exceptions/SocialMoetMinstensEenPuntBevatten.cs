namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class SocialMoetMinstensEenPuntBevatten : DomainException
{
    public SocialMoetMinstensEenPuntBevatten() : base(ExceptionMessages.SocialMediaMissingPeriod)
    {
    }

    protected SocialMoetMinstensEenPuntBevatten(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
