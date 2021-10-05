using System;
using System.Collections.Generic;
using Timberborn.Characters;
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

        private static GlobalPopulation _globalPopulation;
        internal static GlobalPopulation GlobalPopulation
        {
            get => _globalPopulation;
            set
            {
                _globalPopulation = value;
                Plugin.Log.LogDebug($"Updated global pop.");
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

        private static void OnSceneChange(Scene from, Scene to) {
            _masterSceneIsRunning = false;
            Plugin.Log.LogDebug($"Scene changed. Assuming game is not running.");
        }
    }
}