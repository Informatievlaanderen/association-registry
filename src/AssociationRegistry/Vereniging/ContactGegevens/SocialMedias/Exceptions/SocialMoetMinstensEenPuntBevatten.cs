namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
