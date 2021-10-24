using System;
using System.Collections.Generic;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;
using Timberborn.SettlementNameSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;
using UnityEngine.SceneManagement;

namespace VeVantZeData.Collector
{
    static class TimberbornGame
    {
        private static bool _masterSceneIsRunning;

        private static Playthrough _playthrough;
        internal static Playthrough Playthrough
        {
            get => _playthrough;
            set
            {
                _playthrough = value;
                Plugin.Log.LogDebug($"Updated playthrough. Assuming new game has started.");
                _masterSceneIsRunning = true;
                _gameStartActions.ForEach(c => c.Invoke());
            }
        }

        private static string _currentFactionName;
        internal static string CurrentFactionName
        {
            get => _currentFactionName;
            set
            {
                _currentFactionName = value;
                Plugin.Log.LogDebug($"Updated faction name to {value}.");
            }
        }

        private const string _unknownMapName = "UNDETERMINED-MAP";
        private static string _currentMapName = _unknownMapName;
        internal static string CurrentMapName
        {
            get => _currentMapName;
            set
            {
                _currentMapName = value;
                Plugin.Log.LogDebug($"Updated map name to {value}.");
            }
        }

        private static WeatherService _weatherService;
        public static WeatherService WeatherService
        {
            get => _weatherService;
            internal set
            {
                _weatherService = value;
                Plugin.Log.LogDebug($"Updated weather service.");
            }
        }

        private static DayNightCycle _dayNightCycle;
        public static DayNightCycle DayNightCycle
        {
            get => _dayNightCycle;
            internal set
            {
                _dayNightCycle = value;
                Plugin.Log.LogDebug($"Updated day night cycle.");
            }
        }

        private static IEnumerable<GoodSpecification> _goodSpecs;
        public static IEnumerable<GoodSpecification> GoodSpecs
        {
            get => _goodSpecs;
            internal set
            {
                _goodSpecs = value;
                Plugin.Log.LogDebug($"Updated good specs.");
            }
        }

        private static ResourceCountingService _resourceCountingService;
        public static ResourceCountingService ResourceCountingService
        {
            get => _resourceCountingService;
            internal set
            {
                _resourceCountingService = value;
                Plugin.Log.LogDebug($"Updated resource counting service.");
            }
        }

        private static string _settlementName;
        public static string SettlementName
        {
            get => _settlementName;
            internal set
            {
                _settlementName = value;
                Plugin.Log.LogDebug($"Updated settlement name to {value}");
            }
        }

        private static List<Action> _gameStartActions = new List<Action>();

        internal static void AddGameStartCallback(Action callback)
        {
            _gameStartActions.Add(callback);

            SceneManager.activeSceneChanged += OnSceneChange;
        }

        internal static bool IsRunning()
        {
            return _masterSceneIsRunning;
        }

        private static void OnSceneChange(Scene from, Scene to)
        {
            _masterSceneIsRunning = false;
            Plugin.Log.LogDebug($"Scene changed. Assuming game is not running.");
        }

        // the game only bothers with the map name when it starts a new game.
        // new games will thus have the correct map name.
        // loaded games with our playthrough present in the save file will get it from there.
        // loaded games WITHOUT our playthrough will not have it. so it's important to reset it 
        // in order to avoid that the map name from a previous newly started game is used
        private static void ResetMapName()
        {
            _currentMapName = _unknownMapName;
        }
    }
}