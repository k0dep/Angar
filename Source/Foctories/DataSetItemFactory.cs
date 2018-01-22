using Angar.Data;
using Angar.Scripts;

namespace Angar.Factory
{
    public class DataSetItemFactory : IPoolDataSetItemFactory
    {
        public IPoolObjectOverrideData Create()
        {
            return new DataSetItem();
        }
    }
}
