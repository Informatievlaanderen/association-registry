namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;
using DecentraalBeheer.Vereniging;
using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

[DataContract]
public class WijzigKoepelgegevensRequest
{
    /// <summary>
    /// Geeft aan of de vereniging zich als koepel identificeert.
    /// Enkel een vereniging met rechtspersoonlijkheid kan koepel zijn.
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Koepel<br />
    ///     - GeenKoepel<br />
    ///     - NietBepaald<br />
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Koepelstatus { get; set; }

    /// <summary>
    /// Geeft aan of de koepel de privacyverklaring en de toelatingsvoorwaarden heeft geaccepteerd.
    /// Enkel een vereniging met koepelstatus `Koepel` kan de privacyverklaring accepteren.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public bool? PrivacyVerklaringGeaccepteerd { get; set; }

    public WijzigKoepelgegevensCommand ToCommand(string vCode) =>
        new(
            VCode.Create(vCode),
            Koepelstatus is null ? null : DomainKoepelstatus.Parse(Koepelstatus),
            PrivacyVerklaringGeaccepteerd
        );
}
