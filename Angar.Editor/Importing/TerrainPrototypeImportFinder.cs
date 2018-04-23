#if  UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Angar.Data;
using Angar.Factory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar.Importing
{
    public class TerrainPrototypeImportFinder : ISceneObjectImportFinder
    {
        public List<Terrain> Terrains { get; set; }
        public List<GameObject> PrefabsPrototypes { get; set; }

        public IList<GameObject> Prefabs { get { return PrefabsPrototypes; } }

        public TerrainPrototypeImportFinder()
        {
            PrefabsPrototypes = new List<GameObject>();
            Terrains = new List<Terrain>();
        }

        public List<ScenePrefabDataSet> Import(IPoolDataSetFactory dataSetFactory, IPoolDataSetItemFactory dataSetItemFactory)
        {
            var result = new List<ScenePrefabDataSet>();

            var datasets = new Dictionary<GameObject, IDataset>();
            foreach (var prefabsPrototype in PrefabsPrototypes)
            {
                var dataset = dataSetFactory.Create();
                datasets.Add(prefabsPrototype, dataset);
                result.Add(new ScenePrefabDataSet(dataset, prefabsPrototype));
            }

            foreach (var terrain in Terrains)
            {
                var prototypes = new HashSet<int>();

                for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
                {
                    if (PrefabsPrototypes.Contains(terrain.terrainData.treePrototypes[i].prefab))
                        prototypes.Add(i);
                }

                for (int i = 0; i < terrain.terrainData.treeInstanceCount; i++)
                {
                    var newDataSetItem = dataSetItemFactory.Create();

                    var treeInstance = terrain.terrainData.treeInstances[i];
                    if(!prototypes.Contains(treeInstance.prototypeIndex))
                        continue;

                    newDataSetItem.Position = terrain.transform.position +
                                              Vector3.Scale(terrain.terrainData.size, treeInstance.position);

                    newDataSetItem.Rotation = new Vector3(0, treeInstance.rotation, 0);

                    newDataSetItem.Scale = new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);

                    datasets[terrain.terrainData.treePrototypes[treeInstance.prototypeIndex].prefab].Add(newDataSetItem);
                }
            }

            return result;
        }

        public void CollectSceneObjects()
        {
            Terrains = Object.FindObjectsOfType<Terrain>().ToList();

            var prototypesTable = new HashSet<GameObject>();

            foreach (var terrain in Terrains)
            {
                foreach (var terrainDataTreePrototype in terrain.terrainData.treePrototypes)
                {
                    if (!prototypesTable.Contains(terrainDataTreePrototype.prefab))
                        prototypesTable.Add(terrainDataTreePrototype.prefab);
                }
            }

            PrefabsPrototypes = prototypesTable.ToList();
        }

        public void CleanScenes()
        {
            foreach (var terrain in Terrains)
            {
                var prototypes = new HashSet<int>();

                for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
                {
                    if (PrefabsPrototypes.Contains(terrain.terrainData.treePrototypes[i].prefab))
                        prototypes.Add(i);
                }

                var newInstances = new List<TreeInstance>();

                foreach (var terrainDataTreeInstance in terrain.terrainData.treeInstances)
                {
                    if (prototypes.Contains(terrainDataTreeInstance.prototypeIndex))
                        continue;

                    newInstances.Add(terrainDataTreeInstance);
                }

                terrain.terrainData.treeInstances = newInstances.ToArray();
                terrain.Flush();
            }
        }

        public void RemovePrefab(GameObject prefab)
        {
            PrefabsPrototypes.Remove(prefab);
        }
    }
}

#endif