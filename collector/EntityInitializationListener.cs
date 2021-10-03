using System;
using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector
{
    class EntityInitializationListener
    {
        private readonly EventBus _eventBus;

        private readonly List<Action<EntityComponent>> _actions = new List<Action<EntityComponent>>();

        internal EntityInitializationListener(EventBus eventBus, params Action<EntityComponent>[] actions)
        {
            this._eventBus = eventBus;
            _eventBus.Register(this);
            _actions.AddRange(actions);
        }

        internal void TearDown()
        {
            _eventBus.Unregister(this);
            _actions.Clear();
        }

        [OnEvent]
        public void OnEntityInitialization(EntityInitializedEvent @event)
        {
            foreach(var action in _actions)
                action.Invoke(@event.Entity);
        }

    }
}