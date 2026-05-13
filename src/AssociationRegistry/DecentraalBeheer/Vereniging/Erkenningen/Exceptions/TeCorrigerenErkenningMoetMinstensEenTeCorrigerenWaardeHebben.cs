namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben : DomainException
{
    public TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben()
        : base(ExceptionMessages.TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben) { }

    protected TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
