#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Angar.Factory;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Angar.Importing
{
    public class ScenePrefabObjectFinder : ISceneObjectImportFinder
    {
        public Dictionary<GameObject, List<GameObject>> PrefabToObjects { get; set; }

        public ScenePrefabObjectFinder()
        {
            PrefabToObjects = new Dictionary<GameObject, List<GameObject>>();
        }

        public IList<GameObject> Prefabs
        {
            get { return PrefabToObjects.Keys.ToList(); }
        }

        public List<ScenePrefabDataSet> Import(IPoolDataSetFactory dataSetFactory, IPoolDataSetItemFactory dataSetItemFactory)
        {
            var result = new List<ScenePrefabDataSet>();

            foreach (var prefabToObject in PrefabToObjects)
            {
                var dataset = dataSetFactory.Create();
                var newItem = new ScenePrefabDataSet(dataset, prefabToObject.Key);

                foreach (var objectInScene in prefabToObject.Value)
                {
                    var newDataSetItem = dataSetItemFactory.Create();

                    newDataSetItem.Position = objectInScene.transform.position;
                    newDataSetItem.Rotation = objectInScene.transform.rotation.eulerAngles;
                    newDataSetItem.Scale = objectInScene.transform.lossyScale;

                    dataset.Add(newDataSetItem);
                }

                result.Add(newItem);
            }

            return result;
        }

        public void CollectSceneObjects()
        {
            PrefabToObjects.Clear();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                foreach (var rootGameObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    RecursiveCollectObjects(rootGameObject);
                }
            }
        }

        public void CleanScenes()
        {
            foreach (var importerPrefabToObject in PrefabToObjects)
            {
                foreach (var gameObject in importerPrefabToObject.Value)
                {
                    Object.DestroyImmediate(gameObject);
                }
            }
        }

        public void RemovePrefab(GameObject prefab)
        {
            PrefabToObjects.Remove(prefab);
        }

        private void RecursiveCollectObjects(GameObject rootGameObject)
        {
            var prefab = PrefabUtility.GetPrefabParent(rootGameObject) as GameObject;

            if (prefab != null)
            {
                if(!PrefabToObjects.ContainsKey(prefab))
                    PrefabToObjects.Add(prefab, new List<GameObject>());
                
                PrefabToObjects[prefab].Add(rootGameObject);
            }
            else
            {
                foreach (Transform transform in rootGameObject.transform)
                {
                    RecursiveCollectObjects(transform.gameObject);
                }
            }
        }
    }
}

#endif