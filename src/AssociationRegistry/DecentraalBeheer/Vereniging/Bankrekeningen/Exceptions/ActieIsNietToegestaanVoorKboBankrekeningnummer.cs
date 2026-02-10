namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ActieIsNietToegestaanVoorKboBankrekeningnummer : DomainException
{
    public ActieIsNietToegestaanVoorKboBankrekeningnummer()
        : base(ExceptionMessages.UnsupportedOperationForKboBankrekeningnummer) { }

    protected ActieIsNietToegestaanVoorKboBankrekeningnummer(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
