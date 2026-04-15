namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class OngeldigIpdcProductNummer : DomainException
{
    public OngeldigIpdcProductNummer(string ipdcProductNummer)
        : base(string.Format(ExceptionMessages.OngeldigIpdcProductNummer, ipdcProductNummer)) { }

    protected OngeldigIpdcProductNummer(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
