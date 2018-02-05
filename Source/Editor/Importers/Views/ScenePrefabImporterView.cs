#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Angar.Importing;
using EditorViewFramework;
using EditorViewFramework.Controlls;
using UnityEditor;
using UnityEngine;

namespace Angar.Importers.Views
{
    public class ScenePrefabImporterView : Controll
    {
        public ObservableList<PrefabReference> PrefabListModelAccess { get; set; }

        public event Action<GameObject> EventRemovePrefab;
        public event Action EventCollect;
        public event Action EventCreate;
        public event Action<GameObject> PoolRootChanged;
        public event Action<ImportTargets> EventImportTargetChanged;

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
            set
            {
                EditorPrefs.SetString("Angar.ScenePrefabImport.DatasetPrefix", value);
            }
        }

        public string DatasetAssetPrefix { get; set; }

        private ControllVerticalLayout PrefabiList;

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
                if (EventImportTargetChanged != null)
                    EventImportTargetChanged(targets);
            };

            var datasetFolderFile = new ControllTextField("DataSets folder", DatasetFolder);
            datasetFolderFile.EventChange += s => DatasetFolder = s;

            var poolRootName = new ControllObjectField<GameObject>("Pool root");
            poolRootName.EventChange += o => PoolRootChanged(o);

            var dataseAssetPrefix = new ControllTextField("DatsSets asset prefix", DatasetAssetPrefix);
            dataseAssetPrefix.EventChange += s => DatasetAssetPrefix = s;

            PrefabiList = new ControllVerticalLayout();

            var prefabListScroll = new ControllScrollView("box", PrefabiList);

            var buttonCollect = new ControllButton("Select prefabs from scenes");
            buttonCollect.EventClick += () =>
            {
                if (EventCollect != null)
                    EventCollect();
            };

            var buttonCreate = new ControllButton("Create");
            buttonCreate.EventClick += () =>
            {
                if (EventCreate != null)
                    EventCreate();
            };

            MainControll = new ControllVerticalLayout(importTargetClass, datasetFolderFile, poolRootName,
                dataseAssetPrefix, prefabListScroll, buttonCollect, buttonCreate);

            CreateListCollection();
        }

        private void CreateListCollection()
        {
            PrefabiList.NestedControlls.Clear();
            foreach (var prefabReference in PrefabListModelAccess)
            {
                PrefabiList.NestedControlls.Add(new ControllPrefabReference(() =>
                {
                    if (EventRemovePrefab != null)
                        EventRemovePrefab(prefabReference.Prefab);
                },
                prefabReference));
            }
        }


        public class ControllPrefabReference : Controll
        {
            public Action RemoveCallbackAction { get; set; }
            public PrefabReference Reference { get; set; }

            private ControllButton button;
            private ControllObjectFieldAccess<GameObject> obj;
            private ControllHorizontalLayout layout;

            public ControllPrefabReference(Action RemoveAction, PrefabReference reference) : base(null)
            {
                RemoveCallbackAction = RemoveAction;
                Reference = reference;

                button = new ControllButton("Remove", EditorStyles.miniButton);
                button.AddLayoutOptions(GUILayout.MaxWidth(50));

                button.EventClick += RemoveAction;
                obj = new ControllObjectFieldAccess<GameObject>("", () => Reference.Prefab, o => { });
                layout = new ControllHorizontalLayout(EditorStyles.helpBox, button, obj);
            }

            public override void Draw()
            {
                layout.DrawEnabled();
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
            if (Changed != null)
                Changed();
        }

        public new void Remove(TType value)
        {
            base.Remove(value);
            if (Changed != null)
                Changed();
        }

        public new void Clear()
        {
            base.Clear();
            if (Changed != null)
                Changed();
        }
    }
}

#endif