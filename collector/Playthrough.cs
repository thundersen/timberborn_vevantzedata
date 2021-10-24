using System;
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
        internal string MapName { get; private set; }
        internal string FactionName { get; private set; }
        internal string SettlementName { get => TimberbornGame.SettlementName; }

        public Playthrough(ISingletonLoader singletonLoader)
        {
            this._singletonLoader = singletonLoader;
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            var singleton = singletonSaver.GetSingleton(_playthroughKey);
            singleton.Set(_idKey, ID.ToString());
            singleton.Set(_factionNameKey, FactionName);
            singleton.Set(_mapNameKey, MapName);
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
                MapName = TimberbornGame.CurrentMapName;
                FactionName = TimberbornGame.CurrentFactionName;
                ID = Guid.NewGuid();
                Plugin.Log.LogInfo($"Assigning new playthrough ID: {ID} Map: {MapName} Faction: {FactionName}");
            }
            TimberbornGame.Playthrough = this;
        }

        public override string ToString()
        {
            return $"Playthrough: {FactionName} {MapName} {ID}";
        }
    }
}