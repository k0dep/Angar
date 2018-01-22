using UnityEngine;

namespace Angar.Factory
{
    public class DataSetProxyFactory : ITargetableFactory<IPoolDataSetProxy>
    {
        public IPoolDataSetProxy Create(GameObject target)
        {
            var proxy = target.AddComponent<DataSetSource>();
            return proxy;
        }
    }
}
