namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
