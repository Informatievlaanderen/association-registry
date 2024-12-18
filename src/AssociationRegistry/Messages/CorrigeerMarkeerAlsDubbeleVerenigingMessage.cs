namespace AssociationRegistry.Messages;

using Acties.CorrigeerMarkeerAlsDubbel;

public record CorrigeerMarkeerAlsDubbeleVerenigingMessage(string VCode)
{
    public CorrigeerMarkeerAlsDubbeleVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode));
}
