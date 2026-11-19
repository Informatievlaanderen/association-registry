namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;

public record WijzigKoepelgegevensCommand(VCode VCode, Koepelstatus? Koepelstatus, bool? PrivacyVerklaringGeaccepteerd);
