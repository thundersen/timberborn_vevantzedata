using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx;

namespace VeVantZeData.Collector.Output
{
    class CsvWriter : IMetricsOutput
    {
        private static char _slash = Path.DirectorySeparatorChar;
        private static string _outDir = $"{Paths.BepInExRootPath}{_slash}vevantzedata";

        private readonly Playthrough _playthrough;

        internal CsvWriter(Playthrough playthrough)
        {
            _playthrough = playthrough;
        }

        public void Write(Data data)
        {
            Write(GlobalPopsFile(), data.GameTime, data.GlobalPops);
            Write(GlobalGoodsFile(), data.GameTime, data.GlobalStock);

            foreach (var district in data.DistrictPops.Keys)
            {
                WriteDistrictPops(district, data.GameTime, data.DistrictPops[district]);
                WriteDistrictGoods(district, data.GameTime, data.DistrictStocks[district]);
            }
        }

        private string PlaythroughDir()
        {
            var playthroughDir =  Regex.Replace($"{_playthrough.FactionName}_{_playthrough.MapName}_{_playthrough.ID}".ToLower(), @"\s+", "-");
    
            return $"{_outDir}{_slash}{playthroughDir}";
        }

        private string GlobalPopsFile()
        {
            return $"{PlaythroughDir()}{_slash}pops.csv";
        }

        private string GlobalGoodsFile()
        {
            return $"{PlaythroughDir()}{_slash}goods.csv";
        }

        private void WriteDistrictPops(string district, GameTime gameTime, Pops pops)
        {
            var popsFile = $"{DistrictDir(district)}pops.csv";

            Write(popsFile, gameTime, pops);
        }

        private string DistrictDir(string district)
        {
            return $"{PlaythroughDir()}{_slash}{district}{_slash}";
        }

        private void WriteDistrictGoods(string district, GameTime gameTime, Goods goods)
        {
            var goodsFile = $"{DistrictDir(district)}goods.csv";

            Write(goodsFile, gameTime, goods);
        }

        private void Write(string file, GameTime gameTime, Pops pops)
        {
            Write(file, gameTime, new[] { "ADULTS", "CHILDREN", "TOTAL" }, new object[] { pops.Adults, pops.Children, pops.Total });
        }

        private void Write(string file, GameTime gameTime, Goods goods)
        {
            var keysInOrder = goods.Counts.Keys.OrderBy(k => k);

            var columnNames = keysInOrder.Select(k => k.ToUpper()).ToArray();

            var values = keysInOrder.Select(k => (object)goods.Counts[k]).ToArray();

            Write(file, gameTime, columnNames, values);
        }

        private void Write(string file, GameTime gameTime, string[] columnNames, object[] values)
        {
            MakeSureCsvFileExists(file, columnNames);

            WriteLine(file, Concat(ValuesFrom(gameTime), values));
        }

        private void WriteLine(string file, object[] values)
        {
            File.AppendAllLines(file, new[] { LineFrom(values) });
        }

        private object[] ValuesFrom(GameTime gameTime)
        {
            return new object[]
            {
                gameTime.SystemTimeStamp.ToUniversalTime().ToString("o"),
                gameTime.GameTimeStamp.ToString("o"),
                gameTime.Cycle, gameTime.CycleDay, gameTime.TotalDay, gameTime.DayProgress.ToString("n3")
            };
        }

        private object[] Concat(object[] arr1, object[] arr2)
        {
            var result = new object[arr1.Length + arr2.Length];
            arr1.CopyTo(result, 0);
            arr2.CopyTo(result, arr1.Length);
            return result;
        }

        private void MakeSureCsvFileExists(string file, params string[] columns)
        {
            var dir = Path.GetDirectoryName(file);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(file))
                WriteLine(file, Concat(new[] { "TIMESTAMP_SYSTEM", "TIMESTAMP_GAME", "CYCLE", "CYCLEDAY", "TOTALDAY", "DAYPROGRESS" }, columns));

        }

        private string LineFrom(params Object[] values)
        {
            return string.Join(";", values);
        }

    }
}