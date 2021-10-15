using System;
using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector.GameAdapters
{
    class EntityListener
    {
        private static EntityListener _instance;
        internal static void Init(EventBus eventBus)
        {
            if (_instance != null)
                _instance.TearDown();

            _instance = new EntityListener(eventBus,
                new Action<EntityComponent>[] { DistrictCenterListener.CaptureCreatedDistrictCenter },
                new Action<EntityComponent>[] { DistrictCenterListener.CaptureDestroyedDistrictCenter });
        }

        private readonly EventBus _eventBus;
        private readonly List<Action<EntityComponent>> _destructionActions = new List<Action<EntityComponent>>();
        private readonly List<Action<EntityComponent>> _creationActions = new List<Action<EntityComponent>>();

        private EntityListener(EventBus eventBus, Action<EntityComponent>[] creationActions, Action<EntityComponent>[] destructionActions)
        {
            _eventBus = eventBus;
            _eventBus.Register(this);
            _destructionActions.AddRange(destructionActions);
            _creationActions.AddRange(creationActions);
        }

        private void TearDown()
        {
            _eventBus.Unregister(this);
            _creationActions.Clear();
            _destructionActions.Clear();
        }

        [OnEvent]
        public void OnEntityInitialization(EntityInitializedEvent @event)
        {
            foreach (var action in _creationActions)
                action.Invoke(@event.Entity);
        }

        [OnEvent]
        public void OnEntityDestruction(EntityDeletedEvent @event)
        {
            foreach (var action in _destructionActions)
                action.Invoke(@event.Entity);
        }
    }
}