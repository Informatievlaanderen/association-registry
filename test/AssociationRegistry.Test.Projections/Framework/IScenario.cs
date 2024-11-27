namespace AssociationRegistry.Test.Projections.Framework;

public interface IScenario
{
    public string VCode { get; }
    public EventsPerVCode[] Events { get; }
}
