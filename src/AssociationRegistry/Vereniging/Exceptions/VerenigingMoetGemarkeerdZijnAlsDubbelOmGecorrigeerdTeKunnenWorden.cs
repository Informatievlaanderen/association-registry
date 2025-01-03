namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden : DomainException
{
    public VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden() : base(ExceptionMessages.VerenigingMoetGemarkeerdZijnAlsDubbelOmTeKunnenCorrigerenAlsDubbel)
    {
    }

    protected VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
