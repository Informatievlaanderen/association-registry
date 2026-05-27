namespace AssociationRegistry.Scheduled.Host.PowerBi.Exporters;

using System.Collections.ObjectModel;
using AssociationRegistry.Admin.Schema.PowerBiExport;

public class PowerBiExporters : ReadOnlyCollection<Exporter<PowerBiExportDocument>>
{
    public PowerBiExporters(IList<Exporter<PowerBiExportDocument>> exporters)
        : base(exporters)
    {
    }
}
