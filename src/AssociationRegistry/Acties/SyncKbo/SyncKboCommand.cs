namespace AssociationRegistry.Acties.SyncKbo;

using Vereniging;
using Kbo;

public record SyncKboCommand(
    VCode VCode,
    VerenigingVolgensKbo VerenigingVolgensKbo);
