namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
