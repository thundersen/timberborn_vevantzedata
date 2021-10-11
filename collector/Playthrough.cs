using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector
{
    class Playthrough : ISaveableSingleton, ILoadableSingleton
    {
        private static readonly SingletonKey _playthroughKey = new SingletonKey("VeVantZeData.Playthrough");
        private static readonly PropertyKey<string> _mapNameKey = new PropertyKey<string>("MapName");
        private static readonly PropertyKey<string> _factionNameKey = new PropertyKey<string>("FactionName");
        private static readonly PropertyKey<string> _idKey = new PropertyKey<string>("ID");

        private readonly ISingletonLoader _singletonLoader;

        internal Guid ID { get; private set; }
        internal static string MapName { get; set; }
        internal static string FactionName { get; set; }

        public Playthrough(ISingletonLoader singletonLoader)
        {
            this._singletonLoader = singletonLoader;
        }

        internal string ToDirectoryName()
        {
            return Regex.Replace($"{FactionName}_{MapName}_{ID}".ToLower(), @"\s+", "-");
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            var singleton = singletonSaver.GetSingleton(_playthroughKey);
            singleton.Set(_idKey, ID.ToString());
            singleton.Set(_factionNameKey, FactionName ?? "UNKNOWN");
            singleton.Set(_mapNameKey, MapName ?? "UNKNOWN");
        }

        public void Load()
        {
            if (_singletonLoader.HasSingleton(_playthroughKey))
            {
                var singleton = _singletonLoader.GetSingleton(_playthroughKey);
                MapName = singleton.Get(_mapNameKey);
                FactionName = singleton.Get(_factionNameKey);
                ID = Guid.Parse(singleton.Get(_idKey));
            }
            else
            {
                ID = Guid.NewGuid();
                Plugin.Log.LogInfo($"Assigning new playthrough ID: {ID}");
            }
            TimberbornGame.Playthrough = this;
        }

        public override string ToString()
        {
            return $"Playthrough: {FactionName} {MapName} {ID}";
        }
    }
}