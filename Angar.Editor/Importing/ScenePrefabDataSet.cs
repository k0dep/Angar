using Angar.Data;
using UnityEngine;

namespace Angar.Importing
{
    public class ScenePrefabDataSet
    {
        public IDataset DataSet { get; set; }
        public GameObject Prefab { get; set; }

        public ScenePrefabDataSet(IDataset dataSet, GameObject prefab)
        {
            DataSet = dataSet;
            Prefab = prefab;
        }
    }
}