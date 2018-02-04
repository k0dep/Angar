#if UNITY_EDITOR

using System;
using EditorViewFramework;
using EditorViewFramework.Controlls;
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

        private ControllVerticalLayout UpdatersList;

        private Controll MainControll;




        public PoolEditorView(PoolEditorModel model) : base(null)
        {
            Initialize(model);
        }



        public void Initialize(PoolEditorModel model)
        {
            Model = model;

            UpdatersList = new ControllVerticalLayout("CN Box");

            var updatersScroll = new ControllScrollView(UpdatersList);

            var selectAllBtn = new ControllButton("Select all", EditorStyles.toolbarButton);
            selectAllBtn.AddLayoutOptions(GUILayout.MaxWidth(100));

            selectAllBtn.EventClick += SelectAllBtn_EventClick;

            var tollbar= new ControllHorizontalLayout("toolbar", selectAllBtn, new ControllFlexibleSpace());

            var updatersLayout = new ControllVerticalLayout("box", updatersScroll, tollbar);

            var editModeToggle = new ControllToggle("Edit mode", false, "Button");
            editModeToggle.EventChanged += EditModeToggle_EventChanged;

            var proxyUpdateControll = new InternalControllUpdate(new ControllVerticalLayout(updatersLayout, editModeToggle));
            proxyUpdateControll.EventBeforeDraw += UpdateUpdaterList;

            var generalTab = new TabView("Edit mode", proxyUpdateControll);
           
            var removeObjectsBtn = new ControllButton("Remove selected object");
            removeObjectsBtn.EventClick += () =>
            {
                if (EventRemoveObjects != null)
                    EventRemoveObjects();
            };

            var objectsEdit = new TabView("Edit objects", removeObjectsBtn);

            var tabsControll = new ControllTabView(generalTab, objectsEdit);

            MainControll = tabsControll;
        }

        private void UpdateUpdaterList()
        {
            while (UpdatersList.NestedControlls.Count < Model.UpdatersState.Count)
            {
                UpdatersList.NestedControlls.Add(new ControllUpdaterActivity());
            }

            foreach (var updatersListNestedControll in UpdatersList.NestedControlls)
            {
                updatersListNestedControll.Enabled = false;
            }

            for (int i = 0; i < Model.UpdatersState.Count; i++)
            {
                var controll = UpdatersList.NestedControlls[i] as ControllUpdaterActivity;

                controll.Enabled = true;
                controll.UpdaterStateInstance = Model.UpdatersState[i];
                var i1 = i;
                controll.ChangeActiveCallback = () =>
                {
                    if (EventChangeActiveUpdater != null)
                        EventChangeActiveUpdater(Model.UpdatersState[i1].Updater, !Model.UpdatersState[i1].Active);
                };
            }
        }

        private void SelectAllBtn_EventClick()
        {
            if (EventSelectAll != null)
                EventSelectAll();
        }

        private void EditModeToggle_EventChanged(bool obj)
        {
            if (EventChangeEditMode != null)
                EventChangeEditMode(obj);
        }


        public override void Draw()
        {
            MainControll.Draw();
        }


        class InternalControllUpdate : Controll
        {
            public event Action EventBeforeDraw;

            private Controll TargetControll;

            public InternalControllUpdate(Controll target) : base(null)
            {
                TargetControll = target;
            }

            public override void Draw()
            {
                if (EventBeforeDraw != null)
                    EventBeforeDraw();

                TargetControll.DrawEnabled();
            }
        }

        class ControllUpdaterActivity : Controll
        {
            public Action ChangeActiveCallback { get; set; }

            public EditorPoolInstance UpdaterStateInstance { get; set; }

            private Controll layout;

            private ControllToggle toggle;

            public ControllUpdaterActivity() : base(null)
            {
                toggle = new ControllToggle("", false);
                var obj = new ControllObjectFieldAccess<Object>("", () => UpdaterStateInstance.Updater as Object,
                    t => { });
                toggle.AddLayoutOptions(GUILayout.MaxWidth(20));

                layout = new ControllHorizontalLayout("CN Box", toggle, obj);
                layout.AddLayoutOptions(GUILayout.MaxHeight(20));
                layout.AddLayoutOptions(GUILayout.MinHeight(20));
            }

            public override void Draw()
            {
                toggle.Value = UpdaterStateInstance.Active;
                layout.DrawEnabled();

                if (toggle.Value != UpdaterStateInstance.Active && ChangeActiveCallback != null)
                    ChangeActiveCallback();
            }
        }
    }
}
  
#endif