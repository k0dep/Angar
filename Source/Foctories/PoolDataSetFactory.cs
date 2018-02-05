
using Angar.Data;
using UnityEngine;

namespace Angar.Factory
{
    public class PoolDataSetFactory : IPoolDataSetFactory
    {
        public IPoolDataSet Create()
        {
            return ScriptableObject.CreateInstance<PoolDataSet>();
        }
    }
}
