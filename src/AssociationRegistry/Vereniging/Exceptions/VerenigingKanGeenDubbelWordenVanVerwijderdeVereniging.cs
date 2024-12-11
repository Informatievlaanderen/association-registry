namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
