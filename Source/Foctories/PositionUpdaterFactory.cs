using UnityEngine;

namespace Angar.Factory
{
    public class PositionUpdaterFactory : ITargetableFactory<IPositionUpdater>
    {
        public ITargetableFactory EngineFactory { get; set; }

        public PositionUpdaterFactory(ITargetableFactory engineFactory)
        {
            EngineFactory = engineFactory;
        }

        public IPositionUpdater Create(GameObject target)
        {
            var updater = target.AddComponent<PositionUpdater>();

            return updater;
        }
    }
}
