namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd : DomainException
{
    public MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd() : base(ExceptionMessages.InvalidDoelgroepRange)
    {
    }

    protected MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
