namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenDubbelWordenVanDubbelVereniging : DomainException
{
    public VerenigingKanGeenDubbelWordenVanDubbelVereniging() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanDubbelVereniging)
    {
    }

    protected VerenigingKanGeenDubbelWordenVanDubbelVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
