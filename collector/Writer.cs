using System;
using System.IO;
using BepInEx;

namespace VeVantZeData.Collector
{
    class Writer
    {
        private static string _outDir = $"{Paths.BepInExRootPath}{Path.DirectorySeparatorChar}vevantzedata";
        private static string _globalPopsFile = $"{_outDir}{Path.DirectorySeparatorChar}pops.csv";

        internal void Write(Data data)
        {
            Write(_globalPopsFile, data.GlobalPops);

            foreach(var kvp in data.DistrictPops)
            {
                WriteDistrictPops(kvp.Key, kvp.Value);
            }
        }

        private void WriteDistrictPops(string districtName, Pops pops)
        {
            var popsFile = DistrictPopsFileNameFor(districtName);

            Write(popsFile, pops);
        }

        private string DistrictPopsFileNameFor(string district) {
            return $"{_outDir}{Path.DirectorySeparatorChar}{district}{Path.DirectorySeparatorChar}pops.csv";
        }

        private void Write(string file, Pops pops) {
            MakeSureCsvFileExists(file);

            File.AppendAllLines(file, new []{ LineFrom(pops.Adults, pops.Children, pops.Total) });
        }

        private void MakeSureCsvFileExists(string file) {
            var dir = Path.GetDirectoryName(file);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(file))
                File.AppendAllLines(file, new []{LineFrom("ADULTS", "CHILDREN", "TOTAL")});
        }

        private string LineFrom(params Object[] values)
        {
            return string.Join(";", values);
        }

    }
}