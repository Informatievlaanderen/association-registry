namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd : DomainException
{
    public MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd() : base(ExceptionMessages.InvalidDoelgroepRange)
    {
    }

    protected MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
