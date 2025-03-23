namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype : DomainException
{

    public ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype() : base(ExceptionMessages.ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype)
    { }

    protected ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
