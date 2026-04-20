namespace AssociationRegistry.Integrations.Wegwijs.Responses;

using System.Text.Json.Serialization;

public class OrganisationResponse
{
    [JsonPropertyName("changeId")]
    public int ChangeId { get; set; }

    [JsonPropertyName("changeTime")]
    public DateOnly ChangeTime { get; set; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ovoNumber")]
    public string OvoNumber { get; set; } = string.Empty;

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();

    [JsonPropertyName("operationalValidity")]
    public Validity OperationalValidity { get; set; } = new();

    [JsonPropertyName("showOnVlaamseOverheidSites")]
    public bool ShowOnVlaamseOverheidSites { get; set; }

    [JsonPropertyName("keys")]
    public List<OrganisationKey> Keys { get; set; } = [];

    [JsonPropertyName("contacts")]
    public List<OrganisationContact> Contacts { get; set; } = [];

    [JsonPropertyName("organisationClassifications")]
    public List<OrganisationClassification> OrganisationClassifications { get; set; } = [];

    [JsonPropertyName("parents")]
    public List<OrganisationParent> Parents { get; set; } = [];

    [JsonPropertyName("formalFrameworks")]
    public List<OrganisationFormalFramework> FormalFrameworks { get; set; } = [];

    [JsonPropertyName("locations")]
    public List<OrganisationLocation> Locations { get; set; } = [];

    [JsonPropertyName("openingHours")]
    public List<OrganisationOpeningHour> OpeningHours { get; set; } = [];
}

public class Validity
{
    [JsonPropertyName("start")]
    public DateOnly? Start { get; set; }

    [JsonPropertyName("end")]
    public DateOnly? End { get; set; }
}

public class OrganisationKey
{
    [JsonPropertyName("organisationKeyId")]
    public Guid OrganisationKeyId { get; set; }

    [JsonPropertyName("keyTypeId")]
    public Guid KeyTypeId { get; set; }

    [JsonPropertyName("keyTypeName")]
    public string KeyTypeName { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class OrganisationContact
{
    [JsonPropertyName("organisationContactId")]
    public Guid OrganisationContactId { get; set; }

    [JsonPropertyName("contactTypeId")]
    public Guid ContactTypeId { get; set; }

    [JsonPropertyName("contactTypeName")]
    public string ContactTypeName { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class OrganisationClassification
{
    [JsonPropertyName("organisationOrganisationClassificationId")]
    public Guid OrganisationOrganisationClassificationId { get; set; }

    [JsonPropertyName("organisationClassificationTypeId")]
    public Guid OrganisationClassificationTypeId { get; set; }

    [JsonPropertyName("organisationClassificationTypeName")]
    public string OrganisationClassificationTypeName { get; set; } = string.Empty;

    [JsonPropertyName("organisationClassificationId")]
    public Guid OrganisationClassificationId { get; set; }

    [JsonPropertyName("organisationClassificationName")]
    public string OrganisationClassificationName { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class OrganisationParent
{
    [JsonPropertyName("organisationOrganisationParentId")]
    public Guid OrganisationOrganisationParentId { get; set; }

    [JsonPropertyName("parentOrganisationId")]
    public Guid ParentOrganisationId { get; set; }

    [JsonPropertyName("parentOrganisationName")]
    public string ParentOrganisationName { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class OrganisationFormalFramework
{
    [JsonPropertyName("organisationFormalFrameworkId")]
    public Guid OrganisationFormalFrameworkId { get; set; }

    [JsonPropertyName("formalFrameworkId")]
    public Guid FormalFrameworkId { get; set; }

    [JsonPropertyName("formalFrameworkName")]
    public string FormalFrameworkName { get; set; } = string.Empty;

    [JsonPropertyName("parentOrganisationId")]
    public Guid ParentOrganisationId { get; set; }

    [JsonPropertyName("parentOrganisationName")]
    public string ParentOrganisationName { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class OrganisationLocation
{
    [JsonPropertyName("organisationLocationId")]
    public Guid OrganisationLocationId { get; set; }

    [JsonPropertyName("locationId")]
    public Guid LocationId { get; set; }

    [JsonPropertyName("formattedAddress")]
    public string FormattedAddress { get; set; } = string.Empty;

    [JsonPropertyName("components")]
    public LocationComponents Components { get; set; } = new();

    [JsonPropertyName("isMainLocation")]
    public bool IsMainLocation { get; set; }

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}

public class LocationComponents
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("zipCode")]
    public string ZipCode { get; set; } = string.Empty;

    [JsonPropertyName("municipality")]
    public string Municipality { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class OrganisationOpeningHour
{
    [JsonPropertyName("organisationOpeningHourId")]
    public Guid OrganisationOpeningHourId { get; set; }

    [JsonPropertyName("opens")]
    public TimeOnly Opens { get; set; }

    [JsonPropertyName("closes")]
    public TimeOnly Closes { get; set; }

    [JsonPropertyName("dayOfWeek")]
    public string DayOfWeek { get; set; } = string.Empty;

    [JsonPropertyName("validity")]
    public Validity Validity { get; set; } = new();
}
