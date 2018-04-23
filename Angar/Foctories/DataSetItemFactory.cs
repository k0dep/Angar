using Angar.Data;
using Angar.Scripts;

namespace Angar.Factory
{
    public class DataSetItemFactory : IPoolDataSetItemFactory
    {
        public IDatasetItem Create()
        {
            return new DatasetItem();
        }
    }
}
