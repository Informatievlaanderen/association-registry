namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class ActieIsNietToegestaanVoorVerenigingstype : DomainException
{
    public ActieIsNietToegestaanVoorVerenigingstype() : base(ExceptionMessages.UnsupportedOperationForVerenigingstype)
    {
    }

    protected ActieIsNietToegestaanVoorVerenigingstype(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
