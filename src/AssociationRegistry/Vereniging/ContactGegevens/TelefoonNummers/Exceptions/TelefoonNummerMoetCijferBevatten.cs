namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class TelefoonNummerMoetCijferBevatten : DomainException
{
    public TelefoonNummerMoetCijferBevatten() : base(ExceptionMessages.NoNumbersInTelefoonNummer)
    {
    }

    protected TelefoonNummerMoetCijferBevatten(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
