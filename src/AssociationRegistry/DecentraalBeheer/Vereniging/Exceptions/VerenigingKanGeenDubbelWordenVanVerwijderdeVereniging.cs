namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging : DomainException
{
    public VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging)
    {
    }

    protected VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
