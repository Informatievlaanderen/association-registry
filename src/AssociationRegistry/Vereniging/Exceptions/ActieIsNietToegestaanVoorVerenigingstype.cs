namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
