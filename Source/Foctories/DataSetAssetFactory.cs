#if UNITY_EDITOR

using Angar.Data;
using Angar.Factory;
using UnityEditor;
using UnityEngine;

namespace Angar.Foctories
{
    public class DataSetAssetFactory : IPoolDataSetFactory
    {
        public string AssetPath { get; set; }

        public IPoolDataSet Create()
        {
            var dataset = ScriptableObject.CreateInstance<PoolDataSet>();
            AssetDatabase.CreateAsset(dataset, AssetPath);

            return dataset;
        }
    }
}

#endif