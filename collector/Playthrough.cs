using System;
using Timberborn.EntitySystem;

namespace VeVantZeData.Collector
{
    class Playthrough
    {
        private static Playthrough _instance = Default();

        internal Guid ID { get; private set; }

        private Playthrough(Guid id)
        {
            ID = id;
        }

        private Playthrough() { }

        // the game doesn't seem to explicitly track the playthrough.
        // as a work around the ID of the first district center is used.
        // the drawback of this method is that the playthrough ID will change on next load, 
        // if the initial district center is deconstructed.
        internal static Playthrough FromDistrictCenter(EntityComponent dc)
        {
            if (_instance.IsInitialized())
                return _instance;

            Plugin.Log.LogInfo($"Setting ID of first District Center as Playthrough ID: {dc.EntityId}");
            _instance = new Playthrough(dc.EntityId);
            return _instance;
        }

        internal static Playthrough Default()
        {
            return new Playthrough();
        }

        internal bool IsInitialized()
        {
            return (ID != default);
        }
    }
}