namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMoetGemarkeerdZijnAlsDubbelOmTeKunnenCorrigerenAlsDubbel : DomainException
{
    public VerenigingMoetGemarkeerdZijnAlsDubbelOmTeKunnenCorrigerenAlsDubbel() : base(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden)
    {
    }

    protected VerenigingMoetGemarkeerdZijnAlsDubbelOmTeKunnenCorrigerenAlsDubbel(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
