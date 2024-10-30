namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenLidWordenVanVerwijderdeVereniging : DomainException
{
    public VerenigingKanGeenLidWordenVanVerwijderdeVereniging() : base(ExceptionMessages.VerenigingKanGeenLidWordenVanVerwijderdeVereniging)
    {
    }

    protected VerenigingKanGeenLidWordenVanVerwijderdeVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
