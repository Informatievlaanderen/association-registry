namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class RelatieaanvraagVanHetzelfdeTypeStaatAlOpen : DomainException
{
    public RelatieaanvraagVanHetzelfdeTypeStaatAlOpen(RelatieaanvraagId aanvraagId)
        : base(string.Format(ExceptionMessages.RelatieaanvraagVanHetzelfdeTypeStaatAlOpen, aanvraagId.Value)) { }

    protected RelatieaanvraagVanHetzelfdeTypeStaatAlOpen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

[Serializable]
public class RelatieaanvraagVanEenAnderTypeStaatAlOpen : DomainException
{
    public RelatieaanvraagVanEenAnderTypeStaatAlOpen()
        : base(ExceptionMessages.RelatieaanvraagVanEenAnderTypeStaatAlOpen) { }

    protected RelatieaanvraagVanEenAnderTypeStaatAlOpen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

[Serializable]
public class SubverenigingsaanvraagStaatAlOpen : DomainException
{
    public SubverenigingsaanvraagStaatAlOpen(RelatieaanvraagId aanvraagId)
        : base(string.Format(ExceptionMessages.SubverenigingsaanvraagStaatAlOpen, aanvraagId.Value)) { }

    protected SubverenigingsaanvraagStaatAlOpen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

/// <summary>A concurrent relatieaanvraag was saved first: the database allows only one open relatieaanvraag.</summary>
[Serializable]
public class RelatieaanvraagStaatAlOpen : DomainException
{
    public RelatieaanvraagStaatAlOpen()
        : base(ExceptionMessages.RelatieaanvraagStaatAlOpen) { }

    protected RelatieaanvraagStaatAlOpen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
