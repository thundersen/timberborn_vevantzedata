using System;
using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector
{
    class EntityListener
    {
        private readonly EventBus _eventBus;
        private readonly List<Action<EntityComponent>> _destructionActions = new List<Action<EntityComponent>>();
        private readonly List<Action<EntityComponent>> _creationActions = new List<Action<EntityComponent>>();

        internal class Builder{
            private EventBus _eventBus;
            private Action<EntityComponent>[] _creationActions = new Action<EntityComponent>[0];
            private Action<EntityComponent>[] _destructionActions = new Action<EntityComponent>[0];

            public Builder(EventBus eventBus)
            {
                _eventBus = eventBus;
            }

            internal static Builder WithEventBus(EventBus eventBus)
            {
                return new Builder(eventBus);
            }

            internal Builder WithCreationActions(params Action<EntityComponent>[] creationActions)
            {
                _creationActions = creationActions;
                return this;
            }

            internal Builder WithDestructionActions(params Action<EntityComponent>[] destructionActions)
            {
                _destructionActions = destructionActions;
                return this;
            }

            internal EntityListener Build()
            {
                return new EntityListener(_eventBus, _creationActions, _destructionActions);
            }
        }

        internal EntityListener(EventBus eventBus, Action<EntityComponent>[] creationActions, Action<EntityComponent>[] destructionActions)
        {
            _eventBus = eventBus;
            _eventBus.Register(this);
            _destructionActions.AddRange(destructionActions);
            _creationActions.AddRange(creationActions);
        }

        internal void TearDown()
        {
            _eventBus.Unregister(this);
            _creationActions.Clear();
            _destructionActions.Clear();
        }

        [OnEvent]
        public void OnEntityInitialization(EntityInitializedEvent @event)
        {
            foreach(var action in _creationActions)
                action.Invoke(@event.Entity);
        }

        [OnEvent]
        public void OnEntityDestruction(EntityDeletedEvent @event)
        {
            foreach(var action in _destructionActions)
                action.Invoke(@event.Entity);
        }
    }
}