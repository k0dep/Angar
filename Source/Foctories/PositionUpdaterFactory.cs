
using UnityEngine;

namespace Angar.Factory
{
    public class MonoPositionUpdaterFactory : ITargetableFactory<IPositionUpdater>
    {
        public IPositionUpdater Create(GameObject target)
        {
            var updater = target.AddComponent<PositionUpdater>();

            return updater;
        }
    }
}
