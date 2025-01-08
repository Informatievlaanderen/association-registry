namespace AssociationRegistry.Acties.GrarConsumer;

using HeradresseerLocaties;
using OntkoppelAdres;

public record OverkoepelendeGrarConsumerMessage
{
    public HeradresseerLocatiesMessage? HeradresseerLocatiesMessage { get; init; }
    public OntkoppelLocatiesMessage? OntkoppelLocatiesMessage { get; init; }
}
