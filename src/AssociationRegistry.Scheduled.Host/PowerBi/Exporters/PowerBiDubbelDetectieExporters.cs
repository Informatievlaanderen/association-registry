namespace AssociationRegistry.Scheduled.Host.PowerBi.Exporters;

using System.Collections.ObjectModel;
using AssociationRegistry.Admin.Schema.PowerBiExport;

public class PowerBiDubbelDetectieExporters : ReadOnlyCollection<Exporter<PowerBiExportDubbelDetectieDocument>>
{
    public PowerBiDubbelDetectieExporters(IList<Exporter<PowerBiExportDubbelDetectieDocument>> exporters)
        : base(exporters)
    {
    }
}
