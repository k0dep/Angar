#if UNITY_EDITOR

using Angar.Factory;
using Angar.Foctories;
using Angar.Importers.Views;
using Angar.Importing;
using UnityEngine;

namespace Angar.Importers
{
    public class ScenePrefabImporterController
    {
        public ObservableList<PrefabReference> PrefabListModel { get; }

        public ScenePrefabImporterView View { get; }

        public ISceneObjectImportFinder Importer { get; set; }

        public PoolingConfigurationMerger Merger { get; set; }

        public ScenePrefabImporterController(ScenePrefabImporterView view)
        {
            PrefabListModel = new ObservableList<PrefabReference>();
            View = view;

            Merger = new PoolingConfigurationMerger(
                new PoolFactory(),
                new PoolDataSetFactory(),
                new DataSetItemFactory(),
                new DataSetProxyFactory(),
                new PositionUpdaterFactory(null));

            View.Initialize(PrefabListModel);

            View.EventCreate += ViewOnEventCreate;
            View.EventCollect += ViewOnEventCollect;
            View.EventRemovePrefab += ViewOnEventRemovePrefab;
            view.PoolRootChanged += o => Merger.PoolRoot = o;
            View.EventImportTargetChanged += SetFinder;

            var poolRoot = Object.FindObjectOfType<PositionTargetSource>();
            Merger.PoolRoot = View.PoolRoot = poolRoot == null ? null : poolRoot.gameObject;

            SetFinder(View.TargetClass);
        }

        private void SetFinder(ImportTargets targets)
        {
            if(targets == ImportTargets.SceneObjects)
                Importer = new ScenePrefabObjectFinder();
            else
                Importer = new TerrainPrototypeImportFinder();

            RefreshPrefabs();
        }


        private void ViewOnEventRemovePrefab(GameObject gameObject)
        {
            Importer.RemovePrefab(gameObject);
            RefreshPrefabs();
        }


        private void ViewOnEventCreate()
        {
            var importDatasetResult = Importer.Import(new PoolDataSetFactory(), new DataSetItemFactory());

            Merger.PositionUpdaterFactory = new PositionUpdaterFactory(View.EngineSetupFactoryValue);
            var dsFactory = new DataSetAssetFactory();
            Merger.DatasetFactory = dsFactory;
            foreach (var scenePrefabDataSet in importDatasetResult)
            {
                dsFactory.AssetPath = View.DatasetFolder + View.DatasetAssetPrefix + scenePrefabDataSet.Prefab.name + ".asset";
                Merger.Merge(scenePrefabDataSet.Prefab, scenePrefabDataSet.DataSet);
            }

            Importer.CleanScenes();
        }


        private void ViewOnEventCollect()
        {
            Importer.CollectSceneObjects();
            RefreshPrefabs();
        }


        private void RefreshPrefabs()
        {
            
            PrefabListModel.Clear();
            foreach (var importerPrefabToObject in Importer.Prefabs)
            {
                PrefabListModel.Add(new PrefabReference(importerPrefabToObject));
            }
            
        }
    }
}

#endif