using UnityEngine;

namespace Angar.Factory
{
    public class PositionUpdaterFactory : ITargetableFactory<IPositionUpdater>
    {
        public IEngineSetupFactory EngineFactory { get; set; }

        public PositionUpdaterFactory(IEngineSetupFactory engineFactory)
        {
            EngineFactory = engineFactory;
        }

        public IPositionUpdater Create(GameObject target)
        {
            var updater = target.AddComponent<PositionUpdater>();
            EngineFactory.Create(target, updater);
            return updater;
        }
    }
}
