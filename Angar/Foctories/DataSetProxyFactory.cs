using UnityEngine;

namespace Angar.Factory
{
    public class DataSetProxyFactory : ITargetableFactory<IDatasetProxy>
    {
        public IDatasetProxy Create(GameObject target)
        {
            var proxy = target.AddComponent<DatasetSource>();
            return proxy;
        }
    }
}
