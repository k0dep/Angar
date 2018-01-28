using Angar.Data;
using UnityEngine;

namespace Angar
{
    public class PoolObjectEditModeUtility : MonoBehaviour, IPoolObjectEvents
    {
        public IPoolDataSet Dataset { get; private set; }
        public int Index { get; private set; }

        public void PoolDeinitialize()
        {
            var dataset = Dataset[Index];
            dataset.Position = transform.position;
            dataset.Rotation = transform.rotation.eulerAngles;
            dataset.Scale = transform.localScale;
            Dataset[Index] = dataset;
        }

        public void PoolInitialize(int datasetObject, IPoolDataSet dataset, IPoolObjectOverrideData datasetData)
        {
            Dataset = dataset;
            Index = datasetObject;
        }
    }
}
