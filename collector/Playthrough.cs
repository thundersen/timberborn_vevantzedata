using System;
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

        private Guid _id;
        internal static string MapName { private get; set; }
        internal static string FactionName { private get; set; }

        public Playthrough(ISingletonLoader singletonLoader)
        {
            this._singletonLoader = singletonLoader;
        }

        internal string ToDirectoryName()
        {
            return Regex.Replace($"{FactionName}_{MapName}_{_id}".ToLower(), @"\s+", "-");
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            var singleton = singletonSaver.GetSingleton(_playthroughKey);
            singleton.Set(_idKey, _id.ToString());
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
                _id = Guid.Parse(singleton.Get(_idKey));
			} else {
                _id = Guid.NewGuid();
                Plugin.Log.LogInfo($"Assigning new playthrough ID: {_id}");
            }
            TimberbornGame.Playthrough = this;
        }

        public override string ToString()
        {
            return $"Playthrough: {FactionName} {MapName} {_id}";
        }
    }
}