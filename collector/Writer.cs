using System;
using System.IO;
using BepInEx;

namespace VeVantZeData.Collector
{
    class Writer
    {
        private static char _slash = Path.DirectorySeparatorChar;
        private static string _outDir = $"{Paths.BepInExRootPath}{_slash}vevantzedata";

        private Playthrough _playthrough = Playthrough.Default();

        internal void Update(Playthrough playthrough)
        {
            _playthrough = playthrough;

            if (_playthrough.IsInitialized() && !Directory.Exists(PlaythroughDir()))
            {
                Directory.CreateDirectory(PlaythroughDir());
                Plugin.Log.LogInfo($"Created directory {PlaythroughDir()}");
            }
        }

        internal bool IsInitialized()
        {
            return _playthrough.IsInitialized();
        }

        internal void Write(Data data)
        {
            Write(GlobalPopsFile(), data.GlobalPops);

            foreach (var kvp in data.DistrictPops)
            {
                WriteDistrictPops(kvp.Key, kvp.Value);
            }
        }

        private string PlaythroughDir()
        {
            return $"{_outDir}{_slash}{_playthrough.ID}";
        }

        private string GlobalPopsFile()
        {
            return $"{PlaythroughDir()}{_slash}pops.csv";
        }

        private void WriteDistrictPops(string districtName, Pops pops)
        {
            var popsFile = DistrictPopsFileNameFor(districtName);

            Write(popsFile, pops);
        }

        private string DistrictPopsFileNameFor(string district)
        {
            return $"{PlaythroughDir()}{_slash}{district}{_slash}pops.csv";
        }

        private void Write(string file, Pops pops)
        {
            MakeSureCsvFileExists(file);

            File.AppendAllLines(file, new[] { LineFrom(pops.Adults, pops.Children, pops.Total) });
        }

        private void MakeSureCsvFileExists(string file)
        {
            var dir = Path.GetDirectoryName(file);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(file))
                File.AppendAllLines(file, new[] { LineFrom("ADULTS", "CHILDREN", "TOTAL") });
        }

        private string LineFrom(params Object[] values)
        {
            return string.Join(";", values);
        }

    }
}