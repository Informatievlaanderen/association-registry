namespace AssociationRegistry.Framework;

public record CommandEnvelope<TCommand>(TCommand Command, CommandMetadata Metadata);
