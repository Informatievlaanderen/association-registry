namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;
using Framework;
using Marten.Schema;
using NodaTime;

public record Bewaartermijn : IHasVersion
{
    [Identity]
    public string Identity
    {
        get => BewaartermijnId;
        set => BewaartermijnId = value;
    }

    public string BewaartermijnId { get; private set; } = null!;
    public VCode VCode { get; init; } = null!;
    public int VertegenwoordigerId { get; init; }
    public Instant Vervaldag { get; init; }
    public BewaartermijnStatus BewaartermijnStatus { get; init; }
    public long Version { get; set; }

    public Bewaartermijn Apply(BewaartermijnWerdGestart @event)
        => new()
        {
            BewaartermijnId = @event.BewaartermijnId,
            VCode = VCode.Hydrate(@event.VCode),
            VertegenwoordigerId = @event.VertegenwoordigerId,
            Vervaldag = @event.Vervaldag,
            BewaartermijnStatus = BewaartermijnStatus.Gepland,
        };
}
