namespace AssociationRegistry.Grar.Clients.Contracts
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract(Name = "AdresMatchCollectie", Namespace = "")]
    public class AddressMatchOsloCollection
    {
        [DataMember(Name = "@context", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "AdresMatches")]
        public List<AdresMatchOsloItem> AdresMatches { get; set; }
    }

    [DataContract(Name = "AdresMatch", Namespace = "")]
    public class AdresMatchOsloItem
    {
        [DataMember(Name = "@type", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Type => "Adres";

        [DataMember(Name = "Identificator", Order = 1, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public AdresIdentificator? Identificator { get; set; }

        [DataMember(Name = "Detail", Order = 2, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public string Detail { get; set; }

        [DataMember(Name = "Gemeente", Order = 3, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public AdresMatchOsloItemGemeente Gemeente { get; set; }

        [DataMember(Name = "Postinfo", Order = 4, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public AdresMatchOsloItemPostinfo? Postinfo { get; set; }

        [DataMember(Name = "Straatnaam", Order = 5, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public AdresMatchOsloItemStraatnaam Straatnaam { get; set; }

        [DataMember(Name = "Huisnummer", Order = 7, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public string Huisnummer { get; set; }

        [DataMember(Name = "Busnummer", Order = 8, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public string? Busnummer { get; set; }

        [DataMember(Name = "VolledigAdres", Order = 9, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public VolledigAdres VolledigAdres { get; set; }

        [DataMember(Name = "AdresStatus", Order = 13, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public AdresStatus? AdresStatus { get; set; }

        [Range(0.0, 100.0)]
        [DataMember(Name = "Score", Order = 20, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default)]
        public double Score { get; set; }
    }

    [DataContract(Name = "AdresMatchItemGemeente", Namespace = "")]
    public class AdresMatchOsloItemGemeente
    {
        [DataMember(Name = "ObjectId", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ObjectId { get; set; }

        [DataMember(Name = "Detail", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Detail { get; set; }

        [DataMember(Name = "Gemeentenaam", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Gemeentenaam Gemeentenaam { get; set; }
    }

    [DataContract(Name = "AdresMatchItemStraatnaam", Namespace = "")]
    public class AdresMatchOsloItemStraatnaam
    {
        [DataMember(Name = "ObjectId", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ObjectId { get; set; }

        [DataMember(Name = "Detail", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Detail { get; set; }

        [DataMember(Name = "Straatnaam", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Straatnaam Straatnaam { get; set; }
    }

    [DataContract(Name = "Straatnaam", Namespace = "")]
    public class Straatnaam
    {
        [DataMember(Name = "GeografischeNaam")]
        [JsonProperty(Required = Required.DisallowNull)]
        public GeografischeNaam GeografischeNaam { get; set; }
    }

    [DataContract(Name = "AdresMatchItemPostinfo", Namespace = "")]
    public class AdresMatchOsloItemPostinfo
    {
        [DataMember(Name = "ObjectId", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ObjectId { get; set; }

        [DataMember(Name = "Detail", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Detail { get; set; }
    }

    [DataContract(Name = "Gemeentenaam", Namespace = "")]
    public class Gemeentenaam
    {
        [DataMember(Name = "GeografischeNaam")]
        [JsonProperty(Required = Required.DisallowNull)]
        public GeografischeNaam GeografischeNaam { get; set; }
    }

    [DataContract(Name = "GeografischeNaam", Namespace = "")]
    public class GeografischeNaam
    {
        [DataMember(Name = "Spelling", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Spelling { get; set; }

        [DataMember(Name = "Taal", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Taal Taal { get; set; }
    }

    [DataContract(Name = "Taal", Namespace = "")]
    public enum Taal
    {
        [EnumMember(Value = "nl")]
        NL,

        [EnumMember(Value = "fr")]
        FR,

        [EnumMember(Value = "de")]
        DE,

        [EnumMember(Value = "en")]
        EN,
    }

    [DataContract(Name = "AdresStatus", Namespace = "")]
    public enum AdresStatus
    {
        [EnumMember]
        Voorgesteld = 1,

        [EnumMember]
        InGebruik = 2,

        [EnumMember]
        Gehistoreerd = 3,

        [EnumMember]
        Afgekeurd = 4,
    }

    [DataContract(Name = "Identificator", Namespace = "")]
    public class Identificator
    {
        [DataMember(Name = "Id", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Id { get; set; }

        [DataMember(Name = "Naamruimte", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Naamruimte { get; set; }

        [DataMember(Name = "ObjectId", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ObjectId { get; set; }

        [DataMember(Name = "VersieId", Order = 4)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Versie { get; set; }
    }

    [DataContract(Name = "Identificator", Namespace = "")]
    public class AdresIdentificator : Identificator
    {
    }

    [DataContract(Name = "VolledigAdres", Namespace = "")]
    public class VolledigAdres
    {
        /// <summary>
        /// De geografische naam.
        /// </summary>
        [DataMember(Name = "GeografischeNaam")]
        [JsonProperty(Required = Required.DisallowNull)]
        public GeografischeNaam GeografischeNaam { get; set; }

        public VolledigAdres()
        {
        }

        public VolledigAdres(GeografischeNaam geografischeNaam)
        {
            GeografischeNaam = geografischeNaam;
        }

        public VolledigAdres(
            string straatnaam,
            string huisnummer,
            string busnummer,
            string postcode,
            string gemeentenaam,
            Taal taal)
        {
            var representation = string.IsNullOrEmpty(busnummer)
                ? $"{straatnaam} {huisnummer}, {postcode} {gemeentenaam}"
                : $"{straatnaam} {huisnummer} {TranslateBus(taal)} {busnummer}, {postcode} {gemeentenaam}";

            GeografischeNaam = new()
            {
                Spelling = representation,
                Taal = taal,
            };
        }

        private static string TranslateBus(Taal taalCode)
        {
            return taalCode switch
            {
                Taal.DE => "pf",
                Taal.FR => "bte",
                _ => "bus",
            };
        }
    }
}
