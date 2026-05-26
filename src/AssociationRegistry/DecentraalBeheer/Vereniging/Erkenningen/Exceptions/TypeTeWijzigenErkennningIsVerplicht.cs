namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class TypeTeWijzigenErkennningIsVerplicht : DomainException
{
    public TypeTeWijzigenErkennningIsVerplicht()
        : base(ExceptionMessages.TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben) { }

    protected TypeTeWijzigenErkennningIsVerplicht(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
