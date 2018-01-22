using Angar.Data;

namespace Angar
{
    public interface IPoolObjectEvents
    {
        void PoolDeinitialize();
        void PoolInitialize(int datasetObject, IPoolDataSet dataset, IPoolObjectOverrideData datasetData);
    }
}
