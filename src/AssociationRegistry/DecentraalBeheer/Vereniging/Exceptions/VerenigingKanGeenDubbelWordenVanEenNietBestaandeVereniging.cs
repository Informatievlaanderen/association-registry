namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
