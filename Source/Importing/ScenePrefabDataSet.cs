using Angar.Data;
using UnityEngine;

namespace Angar.Importing
{
    public class ScenePrefabDataSet
    {
        public IPoolDataSet DataSet { get; set; }
        public GameObject Prefab { get; set; }

        public ScenePrefabDataSet(IPoolDataSet dataSet, GameObject prefab)
        {
            DataSet = dataSet;
            Prefab = prefab;
        }
    }
}