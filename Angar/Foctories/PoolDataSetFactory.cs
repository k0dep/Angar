
using Angar.Data;
using UnityEngine;

namespace Angar.Factory
{
    public class PoolDataSetFactory : IPoolDataSetFactory
    {
        public IDataset Create()
        {
            return ScriptableObject.CreateInstance<Dataset>();
        }
    }
}
