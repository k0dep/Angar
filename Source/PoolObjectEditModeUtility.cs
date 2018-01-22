using Angar.Data;
using UnityEngine;

namespace Angar
{
    public class PoolObjectEditModeUtility : MonoBehaviour, IPoolObjectEvents
    {
        private IPoolDataSet Dataset;
        private int Index;

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
