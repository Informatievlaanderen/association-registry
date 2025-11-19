namespace AssociationRegistry.Admin.MartenDb.VertegenwoordigerPersoonsgegevens;

using Framework;
using Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensQuery : IQuery<VertegenwoordigerPersoonsgegevensDocument?, VertegenwoordigerPersoonsgegevensByRefIdFilter>,
                                                           IQuery<VertegenwoordigerPersoonsgegevensDocument[], VertegenwoordigerPersoonsgegevensByRefIdsFilter>;
