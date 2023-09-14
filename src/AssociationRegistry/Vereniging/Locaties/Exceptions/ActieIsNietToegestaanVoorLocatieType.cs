namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class ActieIsNietToegestaanVoorLocatieType : DomainException
{
    public ActieIsNietToegestaanVoorLocatieType() : base(ExceptionMessages.UnsupportedOperationForLocatietype)
    {
    }

    protected ActieIsNietToegestaanVoorLocatieType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
