#if UNITY_EDITOR

using Angar.Factory;
using Angar.Foctories;
using Angar.Importers.Views;
using Angar.Importing;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar.Importers
{
    public class ScenePrefabImporterEditor
    {
        [MenuItem("Angar/Importers/Prefab scene importer")]
        public static void Open()
        {
            var view = EditorWindow.GetWindow<ScenePrefabImporterView>(false, "Angar :: Scene prefab importer");
            var Importer = new ScenePrefabImporterEditor(view);
        }

        public ObservableList<PrefabReference> PrefabListModel { get; private set; }

        public ScenePrefabImporterView View { get; private set; }

        public ISceneObjectImportFinder Importer { get; set; }

        public PoolingConfigurationMerger Merger { get; set; }

        public ScenePrefabImporterEditor(ScenePrefabImporterView view)
        {
            PrefabListModel = new ObservableList<PrefabReference>();
            View = view;

            Merger = new PoolingConfigurationMerger(
                new MonoPoolFactory(),
                new MonoPoolDataSetFactory(),
                new DataSetItemFactory(),
                new DataSetProxyFactory(),
                new MonoPositionUpdaterFactory());

            View.Initialize(PrefabListModel);

            View.EventCreate += ViewOnEventCreate;
            View.EventCollect += ViewOnEventCollect;
            View.EventRemovePrefab += ViewOnEventRemovePrefab;
            view.PoolRootChanged += o => Merger.PoolRoot = o;
            View.EventImportTargetChanged += targets => SetFinder(targets);

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
            var importDatasetResult = Importer.Import(new MonoPoolDataSetFactory(), new DataSetItemFactory());

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