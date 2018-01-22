using System.Collections.Generic;
using Angar.Factory;
using UnityEngine;

namespace Angar.Importing
{
    public interface ISceneObjectImportFinder
    {
        IList<GameObject> Prefabs { get; }
        List<ScenePrefabDataSet> Import(IPoolDataSetFactory dataSetFactory, IPoolDataSetItemFactory dataSetItemFactory);
        void CollectSceneObjects();
        void CleanScenes();
        void RemovePrefab(GameObject prefab);
    }
}