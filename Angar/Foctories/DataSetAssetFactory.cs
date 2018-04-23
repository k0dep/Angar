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

        public IDataset Create()
        {
            var dataset = ScriptableObject.CreateInstance<Dataset>();
            AssetDatabase.CreateAsset(dataset, AssetPath);

            return dataset;
        }
    }
}

#endif