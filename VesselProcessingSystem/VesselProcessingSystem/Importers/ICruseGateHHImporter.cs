using VesselProcessingSystem.Models;

namespace VesselProcessingSystem.Importers
{
    public interface ICruseGateHHImporter
    {
        List<Vessel> GetVesselsList();
    }
}
