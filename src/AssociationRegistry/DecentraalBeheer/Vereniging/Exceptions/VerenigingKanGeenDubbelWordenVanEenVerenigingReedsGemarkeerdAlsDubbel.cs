namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel : DomainException
{
    public VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel)
    {
    }

    protected VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
