
using Angar.Data;
using UnityEngine;

namespace Angar.Factory
{
    public class MonoPoolDataSetFactory : IPoolDataSetFactory
    {
        public IPoolDataSet Create()
        {
            return ScriptableObject.CreateInstance<PoolDataSet>();
        }
    }
}
