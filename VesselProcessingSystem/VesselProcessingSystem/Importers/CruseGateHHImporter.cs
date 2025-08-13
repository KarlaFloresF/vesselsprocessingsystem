using ExcelDataReader;
using Microsoft.Extensions.Options;
using VesselProcessingSystem.Models;
using VesselProcessingSystem.Models.Constants;

namespace VesselProcessingSystem.Importers
{
    public class CruseGateHHImporter : ICruseGateHHImporter
    {
        private readonly Configurations _configurations;

        public CruseGateHHImporter(
        IOptions<Configurations> configurations)
        {
            _configurations = configurations.Value;
        }

        public List<Vessel> GetVesselsList()
        {


            var vessels = new List<Vessel>();

            var fileName = "vesselInfo.xlsx";
            var filePath = Path.Combine(AppContext.BaseDirectory, "ImportDocument", fileName);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Archivo no encontrado en: {filePath}");
            }


            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();

            var table = result.Tables[0];

            for (int i = 1; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var vesselType = row[13].ToString();

                if (vesselType.Equals(VesselTypes.Seeschiff) || vesselType.Equals(VesselTypes.Feeder))
                {
                    vessels.Add(new Vessel
                    {
                        Name = row[4].ToString(),
                        VesselType = row[13].ToString(),
                        ETA = row[0].ToString(),
                        ETD = row[11].ToString(),
                        Terminal = row[2].ToString(),
                        ExportTrip = row[6].ToString(),
                        ImportTrip = row[5].ToString(),
                    });
                }
            }

            return vessels;
        }
    }
}
