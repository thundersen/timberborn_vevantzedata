using System;
using System.IO;
using BepInEx;

namespace VeVantZeData.Collector
{
    class Writer
    {
        private static char _slash = Path.DirectorySeparatorChar;
        private static string _outDir = $"{Paths.BepInExRootPath}{_slash}vevantzedata";

        private readonly Playthrough _playthrough;

        internal Writer(Playthrough playthrough)
        {
            _playthrough = playthrough;
        }

        internal void Write(Data data)
        {
            Write(GlobalPopsFile(), data.GameTime, data.GlobalPops);

            foreach (var kvp in data.DistrictPops)
            {
                WriteDistrictPops(kvp.Key, data.GameTime, kvp.Value);
            }
        }

        private string PlaythroughDir()
        {
            return $"{_outDir}{_slash}{_playthrough.ToDirectoryName()}";
        }

        private string GlobalPopsFile()
        {
            return $"{PlaythroughDir()}{_slash}pops.csv";
        }

        private void WriteDistrictPops(string districtName, GameTime gameTime, Pops pops)
        {
            var popsFile = DistrictPopsFileNameFor(districtName);

            Write(popsFile, gameTime, pops);
        }

        private string DistrictPopsFileNameFor(string district)
        {
            return $"{PlaythroughDir()}{_slash}{district}{_slash}pops.csv";
        }

        private void Write(string file, GameTime gameTime, Pops pops)
        {
            MakeSureCsvFileExists(file);

            var line = LineFrom(
                gameTime.TimeStamp.ToUniversalTime().ToString("o"),
                gameTime.Cycle, gameTime.CycleDay, gameTime.TotalDay, gameTime.DayProgress.ToString("n3"),
                pops.Adults, pops.Children, pops.Total
                );

            File.AppendAllLines(file, new[] { line });
        }

        private void MakeSureCsvFileExists(string file)
        {
            var dir = Path.GetDirectoryName(file);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(file))
                File.AppendAllLines(file, new[] { LineFrom("TIMESTAMP", "CYCLE", "CYCLEDAY", "TOTALDAY", "DAYPROGRESS",  "ADULTS", "CHILDREN", "TOTAL") });
        }

        private string LineFrom(params Object[] values)
        {
            return string.Join(";", values);
        }

    }
}