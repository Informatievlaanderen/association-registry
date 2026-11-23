namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel : DomainException
{
    public ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel()
        : base(ExceptionMessages.ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel) { }

    protected ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
