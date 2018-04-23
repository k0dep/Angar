using System.Linq;
using Angar.Data;
using Angar.Factory;
using UnityEngine;

namespace Angar.Importers
{
    public class PoolingConfigurationMerger
    {
        public ITargetableFactory<IPool<GameObject>> PoolFactory { get; set; }
        public IPoolDataSetFactory DatasetFactory { get; set; }
        public IPoolDataSetItemFactory ItemFactory { get; set; }
        public ITargetableFactory<IDatasetProxy> DatasetProxyFactory { get; set; }
        public ITargetableFactory<IPositionUpdater> PositionUpdaterFactory { get; set; }

        public GameObject PoolRoot { get; set; }

        public PoolingConfigurationMerger(ITargetableFactory<IPool<GameObject>> poolFactory,
            IPoolDataSetFactory datasetFactory,
            IPoolDataSetItemFactory itemFactory,
            ITargetableFactory<IDatasetProxy> datasetProxyFactory,
            ITargetableFactory<IPositionUpdater> positionUpdaterFactory)
        {
            PoolFactory = poolFactory;
            DatasetFactory = datasetFactory;
            ItemFactory = itemFactory;
            DatasetProxyFactory = datasetProxyFactory;
            PositionUpdaterFactory = positionUpdaterFactory;
        }

        public void Merge(GameObject prefab, IDataset dataset)
        {
            var pool = GetOrCreatePool(prefab);
            var datasetProxy = GetOrCreateDataSetProxy(pool);
            GetOrCreateUpdater(pool, datasetProxy);

            MergeDataset(datasetProxy, dataset);
        }

        private void MergeDataset(IDataset datasetProxy, IDataset dataset)
        {
            for (int i = 0; i < dataset.Count; i++)
            {
                var item = ItemFactory.Create();
                var source = dataset[i];

                item.Position = source.Position;
                item.Rotation = source.Rotation;
                item.Scale = source.Scale;

                item.SetData(source.GetData());

                datasetProxy.Add(item);
            }
        }

        public IPool<GameObject> GetPoolByPrefab(GameObject prefab)
        {
            return Object
                .FindObjectsOfType<Component>()
                .OfType<IPool<GameObject>>()
                .FirstOrDefault(t => t.Prototype == prefab);
        }

        public IPositionUpdater GetUpdaterByPool(IPool<GameObject> pool)
        {
            var loader = Object
                .FindObjectsOfType<Component>()
                .OfType<IMonoPoolLoader>()
                .FirstOrDefault(t => t.Pool == pool);

            return loader != null ? loader.PositionUpdater : null;
        }

        public IPool<GameObject> GetOrCreatePool(GameObject prefab)
        {
            var existPool = GetPoolByPrefab(prefab);

            if (existPool != null)
                return existPool;

            var objectPool = new GameObject("Pool_" + prefab.name);
            objectPool.transform.SetParent(PoolRoot.transform);
            objectPool.transform.position = Vector3.zero;
            objectPool.transform.rotation = Quaternion.identity;
            objectPool.transform.localScale = Vector3.one;

            existPool = PoolFactory.Create(objectPool);

            existPool.Prototype = prefab;

            return existPool;
        }

        public IDatasetProxy GetOrCreateDataSetProxy(IPool<GameObject> pool)
        {
            var updater = GetUpdaterByPool(pool);

            if (updater != null)
                return updater.DataSet as IDatasetProxy;

            var dataset = DatasetFactory.Create();

            var proxy = DatasetProxyFactory.Create(((Component) pool).gameObject);

            proxy.Origin = dataset;

            return proxy;
        }

        public IPositionUpdater GetOrCreateUpdater(IPool<GameObject> pool, IDataset dataset)
        {
            var updater = GetUpdaterByPool(pool);

            if (updater != null)
                return updater;

            updater = PositionUpdaterFactory.Create(((Component) pool).gameObject);

            updater.DataSet = dataset;
            updater.TargetSource = PoolRoot.GetComponent<IPostionTargetSource>();

            var loader = ((Component) updater).gameObject.AddComponent<PoolLoader>();
            loader.Pool = pool;
            loader.PositionUpdater = updater;

            var enabler = ((Component) updater).gameObject.AddComponent<PoolObjectEnabler>();
            enabler.Pool = pool;

            return updater;
        }
    }

}
