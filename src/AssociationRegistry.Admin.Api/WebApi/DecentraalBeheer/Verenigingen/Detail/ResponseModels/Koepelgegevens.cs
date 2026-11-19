namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Koepelgegevens
{
    /// <summary>
    /// Geeft aan of de vereniging zich als koepel identificeert.
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Koepel<br />
    ///     - GeenKoepel<br />
    ///     - NietBepaald<br />
    /// </summary>
    [DataMember(Name = "Koepelstatus")]
    public string Koepelstatus { get; init; } = null!;

    /// <summary>Geeft aan of de koepel de privacyverklaring en de toelatingsvoorwaarden heeft geaccepteerd.</summary>
    [DataMember(Name = "PrivacyVerklaringGeaccepteerd")]
    public bool PrivacyVerklaringGeaccepteerd { get; init; }
}
