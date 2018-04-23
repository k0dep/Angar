#if UNITY_EDITOR

using Angar.Factory;
using Angar.Importing;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniforms;
using Uniforms.Controlls;
using Uniforms.TypeControlls;
using UnityEditor;
using UnityEngine;

namespace Angar.Importers.Views
{
    public class ScenePrefabImporterView : Controll
    {
        public ObservableList<PrefabReference> PrefabListModelAccess { get; set; }
        public GameObject PoolRoot { get; set; }

        public event Action<GameObject> EventRemovePrefab;
        public event Action EventCollect;
        public event Action EventCreate;
        public event Action<GameObject> PoolRootChanged;
        public event Action<ImportTargets> EventImportTargetChanged;

        public IEngineSetupFactory EngineSetupFactoryValue => (IEngineSetupFactory) _engineSetupFactorySelector.CurrentConstructorParams.Create();

        public ImportTargets TargetClass { get; set; }

        public string DatasetFolder
        {
            get
            {
                if (EditorPrefs.HasKey("Angar.ScenePrefabImport.DatasetPrefix"))
                    return EditorPrefs.GetString("Angar.ScenePrefabImport.DatasetPrefix");

                EditorPrefs.SetString("Angar.ScenePrefabImport.DatasetPrefix", "Assets/Pooling/DataSets");
                return "Assets/Pooling/DataSets";
            }
            set => EditorPrefs.SetString("Angar.ScenePrefabImport.DatasetPrefix", value);
        }

        public string DatasetAssetPrefix { get; set; }

        private ControllVerticalLayout _prefabiList;
        private ControllObjectConstructor _engineSetupFactorySelector;

        public ControllVerticalLayout MainControll { get; set; }

        public ScenePrefabImporterView() : base(null)
        {
        }

        public override void Draw()
        {
            MainControll.Draw();
        }

        public void Initialize(ObservableList<PrefabReference> prefabListModelAccess)
        {
            PrefabListModelAccess = prefabListModelAccess;
            DatasetAssetPrefix = "Dataset_Prefab_";

            PrefabListModelAccess.Changed += CreateListCollection;

            var importTargetClass = new ControllToolbarButtons<ImportTargets>(
                new ControllToolbarButtons<ImportTargets>.ToolbarButtonContent(ImportTargets.SceneObjects,
                    "Scene prefabs"),
                new ControllToolbarButtons<ImportTargets>.ToolbarButtonContent(ImportTargets.Terrains,
                    "Terrain trees"));
            importTargetClass.EventChangeSelected += targets =>
            {
                TargetClass = targets;
                EventImportTargetChanged?.Invoke(targets);
            };
            importTargetClass.Selected = ImportTargets.SceneObjects;
            TargetClass = ImportTargets.SceneObjects;

            var datasetFolderFile = new ControllTextField("DataSets folder", DatasetFolder);
            datasetFolderFile.EventChange += s => DatasetFolder = s;

            var poolRootName = new ControllObjectFieldAccess<GameObject>("Pool root", () => PoolRoot, o => PoolRoot = o);
            poolRootName.EventChange += o =>
            {
                PoolRootChanged?.Invoke(o);
            };

            var dataseAssetPrefix = new ControllTextField("DatsSets asset prefix", DatasetAssetPrefix);
            dataseAssetPrefix.EventChange += s => DatasetAssetPrefix = s;

            var engineSetupFactoryType = typeof(IEngineSetupFactory);
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => engineSetupFactoryType.IsAssignableFrom(t))
                .ToArray();

            _engineSetupFactorySelector = new ControllObjectConstructor("Position engine factory", false, types);

            _prefabiList = new ControllVerticalLayout();

            var prefabListScroll = new ControllScrollView("box", _prefabiList);

            var buttonCollect = new ControllButton("Select prefabs from scenes");
            buttonCollect.EventClick += () =>
            {
                EventCollect?.Invoke();
            };

            var buttonCreate = new ControllButton("Create");
            buttonCreate.EventClick += () =>
            {
                EventCreate?.Invoke();
            };

            MainControll = new ControllVerticalLayout(importTargetClass, datasetFolderFile, poolRootName,
                dataseAssetPrefix, _engineSetupFactorySelector, prefabListScroll, buttonCollect, buttonCreate);

            CreateListCollection();
        }

        private void CreateListCollection()
        {
            _prefabiList.NestedControlls.Clear();
            foreach (var prefabReference in PrefabListModelAccess)
            {
                _prefabiList.NestedControlls.Add(new ControllPrefabReference(() =>
                    {
                        EventRemovePrefab?.Invoke(prefabReference.Prefab);
                    },
                prefabReference));
            }
        }


        public class ControllPrefabReference : Controll
        {
            public Action RemoveCallbackAction { get; set; }
            public PrefabReference Reference { get; set; }

            private readonly ControllHorizontalLayout _layout;

            public ControllPrefabReference(Action removeAction, PrefabReference reference) : base(null)
            {
                RemoveCallbackAction = removeAction;
                Reference = reference;

                var button = new ControllButton("Remove", EditorStyles.miniButton);
                button.AddLayoutOptions(GUILayout.MaxWidth(50));

                button.EventClick += removeAction;
                var obj = new ControllObjectFieldAccess<GameObject>("", () => Reference.Prefab, o => { });
                _layout = new ControllHorizontalLayout(EditorStyles.helpBox, button, obj);
            }

            public override void Draw()
            {
                _layout.DrawEnabled();
            }
        }

    }

    public class PrefabReference
    {
        public GameObject Prefab { get; set; }

        public PrefabReference(GameObject prefab)
        {
            Prefab = prefab;
        }
    }

    public class ObservableList<TType> : List<TType>
    {
        public event Action Changed;

        public new void Add(TType value)
        {
            base.Add(value);
            Changed?.Invoke();
        }

        public new void Remove(TType value)
        {
            base.Remove(value);
            Changed?.Invoke();
        }

        public new void Clear()
        {
            base.Clear();
            Changed?.Invoke();
        }
    }
}

#endif