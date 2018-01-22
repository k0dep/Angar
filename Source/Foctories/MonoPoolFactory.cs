
using UnityEngine;

namespace Angar.Factory
{
    public class MonoPoolFactory : ITargetableFactory<IPool<GameObject>>
    {
        public IPool<GameObject> Create(GameObject target)
        {
            var pool = target.AddComponent<Pool>();

            pool.MaxSize = 10000;
            pool.MinSize = 0;

            return pool;
        }
    }
}
