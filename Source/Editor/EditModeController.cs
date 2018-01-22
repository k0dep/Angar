#if UNITY_EDITOR

using System.Diagnostics;
using Angar.Views;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Angar
{
    public class EditModeController
    {
        [MenuItem("Angar/Object editor(new)")]
        public static void Open()
        {
            var model = new PoolEditorModel();

            var view = EditorWindow.GetWindow<PoolEditorView>(false, "Angar :: Scene editor");
            view.Initialize(model);

            var controller = new EditModeController(model, view);
        }


        private Stopwatch _stopwatch;


        public PoolEditorModel Model { get; set; }
        public PoolEditorView View { get; set; }

        public EditModeController(PoolEditorModel model, PoolEditorView view)
        {
            Model = model;
            View = view;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();


            View.EventSelectAll += View_EventSelectAll;
            View.EventChangeActiveUpdater += ViewOnEventChangeActiveUpdater;
            View.EventChangeEditMode += ViewOnEventChangeEditMode;

            SceneManager.sceneLoaded += EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += EditorSceneManagerOnSceneUnloaded;

            AngarEditorSettings.AddedUpdaterEvent += AngarStaticSettingsOnAddedUpdaterEvent;
            AngarEditorSettings.RemovedUpdaterEvent += AngarStaticSettingsOnRemovedUpdaterEvent;

            EditorApplication.update += Update;

            Model.RefreshUpdaters();
        }

        ~EditModeController()
        {
            SceneManager.sceneLoaded -= EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded -= EditorSceneManagerOnSceneUnloaded;

            AngarEditorSettings.AddedUpdaterEvent -= AngarStaticSettingsOnAddedUpdaterEvent;
            AngarEditorSettings.RemovedUpdaterEvent -= AngarStaticSettingsOnRemovedUpdaterEvent;

            EditorApplication.update -= Update;
        }


        private void ViewOnEventChangeEditMode(bool b)
        {
            if (b)
                Model.EnterEditMode();
            else
                Model.ExitEditMode();
        }

        private void ViewOnEventChangeActiveUpdater(IPositionUpdater positionUpdater1, bool b)
        {
            Model.SetUpdaterActivity(positionUpdater1, b);
        }

        private void View_EventSelectAll()
        {
            foreach (var editorPoolInstance in Model.UpdatersState)
            {
                Model.SetUpdaterActivity(editorPoolInstance.Updater, true);
            }
        }

        private void Update()
        {
            Model.Update(_stopwatch.ElapsedMilliseconds / 1000.0f);
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        private void AngarStaticSettingsOnRemovedUpdaterEvent(IPositionUpdater positionUpdater1)
        {
            Model.RemoveUpdater(positionUpdater1);
        }

        private void AngarStaticSettingsOnAddedUpdaterEvent(IPositionUpdater positionUpdater1)
        {
            Model.AddUpdater(positionUpdater1);
        }

        private void EditorSceneManagerOnSceneUnloaded(Scene arg0)
        {
            Model.RefreshUpdaters();
        }

        private void EditorSceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Model.RefreshUpdaters();
        }

    }
}

#endif