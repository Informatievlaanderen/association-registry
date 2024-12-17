namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging : DomainException
{
    public VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging)
    {
    }

    protected VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
