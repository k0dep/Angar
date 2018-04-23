#if UNITY_EDITOR

using System;
using Angar.Importers.Views;
using Uniforms;
using Uniforms.Controlls;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar.Views
{
    public class PoolEditorView : Controll
    {
        public PoolEditorModel Model { get; set; }

        public event Action EventSelectAll;
        public event Action<IPositionUpdater, bool> EventChangeActiveUpdater;
        public event Action<bool> EventChangeEditMode;
        public event Action EventRemoveObjects;
        public event Action EventCreateRoot;

        private ControllVerticalLayout _updatersList;

        private Controll _mainControll;
        private Controll _createRootControll;



        public PoolEditorView(PoolEditorModel model, ScenePrefabImporterView scenePrefabImporterView) : base(null)
        {
            Initialize(model, scenePrefabImporterView);
        }



        public void Initialize(PoolEditorModel model, ScenePrefabImporterView sceneImportView)
        {
            Model = model;

            _updatersList = new ControllVerticalLayout("CN Box");

            var updatersScroll = new ControllScrollView(_updatersList);

            var selectAllBtn = new ControllButton("Select all", EditorStyles.toolbarButton);
            selectAllBtn.AddLayoutOptions(GUILayout.MaxWidth(100));

            selectAllBtn.EventClick += SelectAllBtn_EventClick;

            var tollbar= new ControllHorizontalLayout("toolbar", selectAllBtn, new ControllFlexibleSpace());

            var updatersLayout = new ControllVerticalLayout("box", updatersScroll, tollbar);

            var editModeToggle = new ControllToggle("Edit mode", false, "Button");
            editModeToggle.EventChanged += EditModeToggle_EventChanged;

            var proxyUpdateControll = new InternalControllUpdate(new ControllVerticalLayout(updatersLayout, editModeToggle));
            proxyUpdateControll.EventBeforeDraw += UpdateUpdaterList;


            // create tabs view

            var generalTab = new TabView("Edit mode", proxyUpdateControll);
           
            var removeObjectsBtn = new ControllButton("Remove selected object");
            removeObjectsBtn.EventClick += () =>
            {
                EventRemoveObjects?.Invoke();
            };


            var objectsEdit = new TabView("Edit objects", removeObjectsBtn);

            var importingButton = new TabView("Scene importing", sceneImportView);

            var tabsControll = new ControllTabView(generalTab, objectsEdit, importingButton);

            _mainControll = tabsControll;

            var createBtn = new ControllButton("Create root");
            createBtn.EventClick += () =>
            {
                EventCreateRoot?.Invoke();
            };

            _createRootControll = createBtn;
        }

        private void UpdateUpdaterList()
        {
            while (_updatersList.NestedControlls.Count < Model.UpdatersState.Count)
            {
                _updatersList.NestedControlls.Add(new ControllUpdaterActivity());
            }

            foreach (var updatersListNestedControll in _updatersList.NestedControlls)
            {
                updatersListNestedControll.Enabled = false;
            }

            for (int i = 0; i < Model.UpdatersState.Count; i++)
            {
                if (_updatersList.NestedControlls[i] is ControllUpdaterActivity controll)
                {
                    controll.Enabled = true;
                    controll.UpdaterStateInstance = Model.UpdatersState[i];
                    var i1 = i;
                    controll.ChangeActiveCallback = () =>
                    {
                        EventChangeActiveUpdater?.Invoke(Model.UpdatersState[i1].Updater,
                            !Model.UpdatersState[i1].Active);
                    };
                }
            }
        }

        private void SelectAllBtn_EventClick()
        {
            EventSelectAll?.Invoke();
        }

        private void EditModeToggle_EventChanged(bool obj)
        {
            EventChangeEditMode?.Invoke(obj);
        }


        public override void Draw()
        {
            if(Model.ExistAngarRoot)
                _mainControll.Draw();
            else
                _createRootControll.Draw();
        }


        class InternalControllUpdate : Controll
        {
            public event Action EventBeforeDraw;

            private readonly Controll _targetControll;

            public InternalControllUpdate(Controll target) : base(null)
            {
                _targetControll = target;
            }

            public override void Draw()
            {
                EventBeforeDraw?.Invoke();

                _targetControll.DrawEnabled();
            }
        }

        class ControllUpdaterActivity : Controll
        {
            public Action ChangeActiveCallback { get; set; }

            public EditorPoolInstance UpdaterStateInstance { get; set; }

            private readonly Controll _layout;

            private readonly ControllToggle _toggle;

            public ControllUpdaterActivity() : base(null)
            {
                _toggle = new ControllToggle("", false);
                var obj = new ControllObjectFieldAccess<Object>("", () => UpdaterStateInstance.Updater as Object,
                    t => { });
                _toggle.AddLayoutOptions(GUILayout.MaxWidth(20));

                _layout = new ControllHorizontalLayout("CN Box", _toggle, obj);
                _layout.AddLayoutOptions(GUILayout.MaxHeight(20));
                _layout.AddLayoutOptions(GUILayout.MinHeight(20));
            }

            public override void Draw()
            {
                _toggle.Value = UpdaterStateInstance.Active;
                _layout.DrawEnabled();

                if (_toggle.Value != UpdaterStateInstance.Active && ChangeActiveCallback != null)
                    ChangeActiveCallback();
            }
        }
    }
}
  
#endif